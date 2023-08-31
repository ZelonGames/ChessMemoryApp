using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessMemoryApp.Model.CourseMaker;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class King : Piece
    {
        public const string KingSideCastle = "O-O";
        public const string QueenSideCastle = "O-O-O";

        public King(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'k')
        {

        }

        public static HashSet<string> GetAvailableMoves(string pieceLetterCoordinates, string fen)
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(pieceLetterCoordinates);

            char? king = FenHelper.GetPieceOnSquare(fen, pieceLetterCoordinates);
            bool isInCheck = IsSquareControlledByEnemy(pieceLetterCoordinates, pieceLetterCoordinates, fen);

            if (!isInCheck)
            {
                bool isWhite = char.IsUpper(king.Value);
                string row = isWhite ? "1" : "8";

                if (FenHelper.CanWhiteCastleKingSide(fen) && !IsKingSideCoveredByEnemy(pieceLetterCoordinates, fen))
                    availableMoves.Add("g" + row);
                if (FenHelper.CanWhiteCastleQueenSide(fen) && !IsQueenSideCoveredByEnemy(pieceLetterCoordinates, fen))
                    availableMoves.Add("c" + row);
            }

            // Right
            string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, pieceCoordinates.Y + 1));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Down
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, pieceCoordinates.Y - 1));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Up Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y + 1));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Up Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y + 1));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Down Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y - 1));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Down Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y - 1));
            if (!IsSquareControlledByEnemy(currentCoordinates, pieceLetterCoordinates, fen))
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            return availableMoves;
        }

        public static bool IsKingSideCoveredByEnemy(string pieceLetterCoordinates, string fen)
        {
            char? king = FenHelper.GetPieceOnSquare(fen, pieceLetterCoordinates);
            ColorType enemyColor = GetOppositeColor(GetColorOfPiece(king.Value));
            Dictionary<string, char?> enemyPieces = FenHelper.GetPiecesByColorFromFen(fen, enemyColor);
            string row = char.IsUpper(king.Value) ? "1" : "8";

            foreach (var piece in enemyPieces)
            {
                HashSet<string> availableMoves = GetAvailableMoves(piece.Value.Value, piece.Key, fen);
                if (availableMoves.Contains("f" + row) ||
                    availableMoves.Contains("g" + row))
                    return true;
            }

            return false;
        }

        public static bool IsQueenSideCoveredByEnemy(string pieceLetterCoordinates, string fen)
        {
            char? king = FenHelper.GetPieceOnSquare(fen, pieceLetterCoordinates);
            ColorType enemyColor = GetOppositeColor(GetColorOfPiece(king.Value));
            Dictionary<string, char?> enemyPieces = FenHelper.GetPiecesByColorFromFen(fen, enemyColor);
            string row = char.IsUpper(king.Value) ? "1" : "8";

            foreach (var piece in enemyPieces)
            {
                HashSet<string> availableMoves = GetAvailableMoves(piece.Value.Value, piece.Key, fen);
                if (availableMoves.Contains("d" + row) ||
                    availableMoves.Contains("c" + row))
                    return true;
            }

            return false;
        }

        public static bool IsSquareControlledByEnemy(string squareCoordinates, string pieceLetterCoordinates, string fen)
        {
            char? king = FenHelper.GetPieceOnSquare(fen, pieceLetterCoordinates);
            ColorType enemyColor = GetOppositeColor(GetColorOfPiece(king.Value));
            Dictionary<string, char?> enemyPieces = FenHelper.GetPiecesByColorFromFen(fen, enemyColor);

            foreach (var piece in enemyPieces)
            {
                HashSet<string> availableMoves = GetAvailableMoves(piece.Value.Value, piece.Key, fen);
                if (availableMoves.Contains(squareCoordinates))
                    return true;
            }

            return false;
        }
    }
}
