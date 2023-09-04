using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class ThreatEngine
    {
        private FamilyTree<string> tree = new();
        private List<List<FamilyTree<string>>> lines;
        public readonly int depth;

        public ThreatEngine(int depth)
        {
            this.depth = depth;
        }

        public void CalculateMoves(string fen)
        {
            CalculateMovesRecurrsive(fen, tree);
        }

        public List<List<FamilyTree<string>>> GetLines()
        {
            if (lines != null)
                return lines;
            return lines = tree.GetPaths();
        }

        private void CalculateMovesRecurrsive(string fen, FamilyTree<string> parent)
        {
            List<string> threats = GetThreats(fen);
            for (int i = 0; i < threats.Count; i++)
            {
                string threat = threats[i];
                FamilyTree<string> child = parent.AddChild();
                child.value = threat;
            }

            for (int i = 0; i < parent.Children.Count; i++)
            {
                FamilyTree<string> child = parent.Children[i];
                string newFen = FenHelper.MakeMoveWithCoordinates(fen, child.value);
                if (child.Depth <= depth)
                    CalculateMovesRecurrsive(newFen, child);
            }
        }

        public static HashSet<string> GetChecks(string fen)
        {
            var checks = new HashSet<string>();
            Piece.ColorType colorToPlay = FenHelper.GetColorToPlayFromFen(fen) == "w" ? Piece.ColorType.White : Piece.ColorType.Black;
            Dictionary<string, char> friendlyPieces = FenHelper.GetPiecesByColorFromFen(fen, colorToPlay);

            char enemyKingType = colorToPlay == Piece.ColorType.White ? 'k' : 'K';
            KeyValuePair<string, char> enemyKing = FenHelper.GetPiecesOfTypeFromFen(enemyKingType, fen).FirstOrDefault();
            if (enemyKing.Key == null)
                return checks;

            string enemyKingCoordinates = enemyKing.Key;

            foreach (var piece in friendlyPieces)
            {
                string fromCoordinates = piece.Key;
                HashSet<string> availableMoves = Piece.GetAvailableMoves(piece.Value, piece.Key, fen);

                foreach (var toCoordinates in availableMoves)
                {
                    string moveNotation = fromCoordinates + toCoordinates;
                    string newFen = FenHelper.MakeMoveWithCoordinates(fen, moveNotation);

                    // Loop through the pieces again to find discovered checks
                    Dictionary<string, char> newPieces = FenHelper.GetPiecesByColorFromFen(newFen, colorToPlay);
                    foreach (var newPiece in newPieces)
                    {
                        HashSet<string> newAvailableMoves = Piece.GetAvailableMoves(newPiece.Value, newPiece.Key, newFen);

                        if (newAvailableMoves.Contains(enemyKingCoordinates))
                            checks.Add(moveNotation);
                    }
                }
            }

            return checks;
        }

        public static HashSet<string> GetCaptures(string fen)
        {
            var captures = new HashSet<string>();
            Piece.ColorType colorToPlay = FenHelper.GetColorToPlayFromFen(fen) == "w" ? Piece.ColorType.White : Piece.ColorType.Black;
            Dictionary<string, char> friendlyPieces = FenHelper.GetPiecesByColorFromFen(fen, colorToPlay);

            foreach (var piece in friendlyPieces)
            {
                HashSet<string> availableMoves = Piece.GetAvailableMoves(piece.Value, piece.Key, fen);
                string fromCoordinates = piece.Key;

                foreach (var toCoordinates in availableMoves)
                {
                    string moveNotation = fromCoordinates + toCoordinates;

                    char? capturePiece = FenHelper.GetPieceOnSquare(fen, toCoordinates);
                    if (capturePiece.HasValue)
                        captures.Add(moveNotation);
                }
            }

            return captures;
        }

        public static List<string> GetThreats(string fen)
        {
            var threats = new HashSet<string>();
            threats.UnionWith(GetChecks(fen));
            threats.UnionWith(GetCaptures(fen));

            Piece.ColorType colorToPlay = FenHelper.GetColorTypeToPlayFromFen(fen);
            Dictionary<string, char> friendlyPieces = FenHelper.GetPiecesByColorFromFen(fen, colorToPlay);

            foreach (var piece in friendlyPieces)
            {
                string fromCoordinates = piece.Key;
                HashSet<string> availableMoves = Piece.GetAvailableMoves(piece.Value, fromCoordinates, fen);
                Dictionary<string, AttackedPiece> oldAttackedPieces = GetAttackedPiecesFromFen(fen);
                Dictionary<string, AttackedPiece> oldAttackedPiecesByPiece = GetAttackedPiecesByPiece(fen, fromCoordinates);

                foreach (var toCoordinates in availableMoves)
                {
                    string moveNotation = fromCoordinates + toCoordinates;
                    string newFen = FenHelper.MakeMoveWithCoordinates(fen, moveNotation, false);
                    Dictionary<string, AttackedPiece> newAttackedPieces = GetAttackedPiecesFromFen(newFen);
                    Dictionary<string, AttackedPiece> newAttackedPiecesByPiece = GetAttackedPiecesByPiece(newFen, toCoordinates);

                    #region Check if new pieces are being attacked
                    foreach (var newAttackedPiece in newAttackedPieces)
                    {
                        if (!oldAttackedPieces.ContainsKey(newAttackedPiece.Key))
                        {
                            if (!threats.Contains(moveNotation))
                                threats.Add(moveNotation);
                        }
                    }
                    #endregion

                    #region Check if moved piece is attacking new pieces
                    foreach (var newAttackedPiece in newAttackedPiecesByPiece)
                    {
                        if (!oldAttackedPiecesByPiece.ContainsKey(newAttackedPiece.Key))
                        {
                            if (!threats.Contains(moveNotation))
                                threats.Add(moveNotation);
                        }
                    }
                    #endregion
                }
            }

            return threats.ToList();
        }

        private static Dictionary<string, AttackedPiece> GetAttackedPiecesFromFen(string fen)
        {
            var attackedPieces = new Dictionary<string, AttackedPiece>();
            Piece.ColorType colorToPlay = FenHelper.GetColorTypeToPlayFromFen(fen);
            Dictionary<string, char> friendlyPieces = FenHelper.GetPiecesByColorFromFen(fen, colorToPlay);
            Dictionary<string, char> enemyPieces = FenHelper.GetPiecesByColorFromFen(fen, Piece.GetOppositeColor(colorToPlay));

            foreach (var friendlyPiece in friendlyPieces)
            {
                var availableMoves = Piece.GetAvailableMoves(friendlyPiece.Value, friendlyPiece.Key, fen);

                foreach (var enemyPiece in enemyPieces)
                {
                    if (!availableMoves.Contains(enemyPiece.Key))
                        continue;

                    if (attackedPieces.ContainsKey(enemyPiece.Key))
                        attackedPieces[enemyPiece.Key].TryAddThreateningPiece(friendlyPiece.Key, friendlyPiece.Value);
                    else
                    {
                        var attackedPiece = new AttackedPiece(enemyPiece.Value);
                        attackedPiece.TryAddThreateningPiece(friendlyPiece.Key, friendlyPiece.Value);
                        attackedPieces.Add(enemyPiece.Key, attackedPiece);
                    }
                }
            }

            return attackedPieces;
        }

        private static Dictionary<string, AttackedPiece> GetAttackedPiecesByPiece(string fen, string pieceCoordinates)
        {
            var attackedPieces = new Dictionary<string, AttackedPiece>();
            char? piece = FenHelper.GetPieceOnSquare(fen, pieceCoordinates);
            if (!piece.HasValue)
                return attackedPieces;

            HashSet<string> availableMoves = Piece.GetAvailableMoves(piece.Value, pieceCoordinates, fen);
            Piece.ColorType enemyColor = Piece.GetOppositeColor(FenHelper.GetColorTypeToPlayFromFen(fen));
            Dictionary<string, char> enemyPieces = FenHelper.GetPiecesByColorFromFen(fen, enemyColor);

            foreach (var enemyPiece in enemyPieces)
            {
                if (availableMoves.Contains(enemyPiece.Key))
                {
                    var attackedPiece = new AttackedPiece(enemyPiece.Value);
                    attackedPiece.TryAddThreateningPiece(pieceCoordinates, piece.Value);
                    attackedPieces.Add(enemyPiece.Key, attackedPiece);
                }
            }

            return attackedPieces;
        }

    }
}
