using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class ThreatEngine
    {
        private Dictionary<string, int> repeatedPositions = new();
        private FamilyTree<CalculatedMoveInfo> tree = new();
        private List<Stack<FamilyTree<CalculatedMoveInfo>>> lines;
        private readonly ChessboardGenerator chessboard;
        public readonly int depth;

        public ThreatEngine(ChessboardGenerator chessboard, int depth)
        {
            this.chessboard = chessboard;
            this.depth = depth;
        }

        public void CalculateMoves(string fen)
        {
            CalculateMovesRecursive(fen, tree);
        }

        public List<Stack<FamilyTree<CalculatedMoveInfo>>> GetLines()
        {
            if (lines != null)
                return lines;
            return lines = tree.GetPaths();
        }

        private void CalculateMovesRecursive(string fen, FamilyTree<CalculatedMoveInfo> parent)
        {
            if (IsDrawnByRepetition(fen))
                return;

            List<string> threats = GetThreats(fen);
            for (int i = 0; i < threats.Count; i++)
            {
                string threat = threats[i];
                parent.AddChild(new CalculatedMoveInfo(fen, threat));
            }

            for (int i = 0; i < parent.Children.Count; i++)
            {
                FamilyTree<CalculatedMoveInfo> child = parent.Children[i];
                string newFen = FenHelper.MakeMoveWithCoordinates(fen, child.value.moveNotationCoordinates);
                if (IsDrawnByRepetition(newFen))
                    continue;

                if (child.Depth <= depth)
                    CalculateMovesRecursive(newFen, child);
            }
        }

        private bool IsDrawnByRepetition(string fen)
        {
            if (repeatedPositions.ContainsKey(fen))
            {
                if (repeatedPositions[fen] == 3)
                    return true;
                repeatedPositions[fen]++;
            }
            else
                repeatedPositions.Add(fen, 1);

            return false;
        }

        public static string ConvertLineToString(Stack<FamilyTree<CalculatedMoveInfo>> line)
        {
            string lineString = "";

            foreach (var moveInfo in line)
                lineString += moveInfo.value.moveNotationCoordinates + " ";

            return lineString;
        }

        public List<string> GetThreats(string fen)
        {
            chessboard.LoadPiecesFromFen(fen);

            var threats = new HashSet<string>();
            Piece.ColorType colorToPlay = FenHelper.GetColorTypeToPlayFromFen(fen);
            List<Piece> friendlyPieces = chessboard.GetPiecesByColor(colorToPlay);

            foreach (var piece in friendlyPieces)
            {
                string fromCoordinates = piece.coordinates;
                HashSet<string> availableMoves = piece.GetAvailableMoves();
                Dictionary<string, AttackedPiece> oldAttackedPieces = GetAttackedPieces();
                Dictionary<string, AttackedPiece> oldAttackedPiecesByPiece = GetAttackedPiecesByPiece(chessboard.GetPiece(fromCoordinates));

                foreach (var toCoordinates in availableMoves)
                {
                    string moveNotation = fromCoordinates + toCoordinates;
                    string newFen = FenHelper.MakeMoveWithCoordinates(fen, moveNotation, false);
                    Dictionary<string, AttackedPiece> newAttackedPieces = GetAttackedPieces();
                    Dictionary<string, AttackedPiece> newAttackedPiecesByPiece = GetAttackedPiecesByPiece(chessboard.GetPiece(toCoordinates));

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

        private Dictionary<string, AttackedPiece> GetAttackedPieces()
        {
            var attackedPieces = new Dictionary<string, AttackedPiece>();
            Piece.ColorType colorToPlay = chessboard.boardColorOrientation;
            List<Piece> friendlyPieces = chessboard.GetPiecesByColor(colorToPlay);
            List<Piece> enemyPieces = chessboard.GetPiecesByColor(Piece.GetOppositeColor(colorToPlay));

            foreach (var friendlyPiece in friendlyPieces)
            {
                var availableMoves = friendlyPiece.GetAvailableMoves();

                foreach (var enemyPiece in enemyPieces)
                {
                    if (!availableMoves.Contains(enemyPiece.coordinates))
                        continue;

                    if (attackedPieces.ContainsKey(enemyPiece.coordinates))
                        attackedPieces[enemyPiece.coordinates].TryAddThreateningPiece(friendlyPiece.coordinates, friendlyPiece.pieceType);
                    else
                    {
                        var attackedPiece = new AttackedPiece(enemyPiece.pieceType);
                        attackedPiece.TryAddThreateningPiece(friendlyPiece.coordinates, friendlyPiece.pieceType);
                        attackedPieces.Add(enemyPiece.coordinates, attackedPiece);
                    }
                }
            }

            return attackedPieces;
        }

        private Dictionary<string, AttackedPiece> GetAttackedPiecesByPiece(Piece piece)
        {
            var attackedPieces = new Dictionary<string, AttackedPiece>();
            if (piece == null)
                return attackedPieces;

            HashSet<string> availableMoves = piece.GetAvailableMoves();
            Piece.ColorType enemyColor = Piece.GetOppositeColor(piece.color);
            List<Piece> enemyPieces = chessboard.GetPiecesByColor(enemyColor);

            foreach (var enemyPiece in enemyPieces)
            {
                if (availableMoves.Contains(enemyPiece.coordinates))
                {
                    var attackedPiece = new AttackedPiece(enemyPiece.pieceType);
                    attackedPiece.TryAddThreateningPiece(piece.coordinates, piece.pieceType);
                    attackedPieces.Add(enemyPiece.coordinates, attackedPiece);
                }
            }

            return attackedPieces;
        }

    }
}
