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

        public King(ChessboardGenerator chessBoard, char pieceType) : base(chessBoard, pieceType)
        {

        }

        public override HashSet<string> GetAvailableMoves()
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(coordinates);

            bool isInCheck = IsInCheck();

            if (!isInCheck)
            {
                string row = color == ColorType.White ? "1" : "8";
                bool canCastleKingSide = color == ColorType.White ? chessBoard.fenSettings.CanWhiteCastleKingSide : chessBoard.fenSettings.CanBlackCastleKingSide;
                bool canCastleQueenSide = color == ColorType.White ? chessBoard.fenSettings.CanWhiteCastleQueenSide : chessBoard.fenSettings.CanBlackCastleQueenSide;

                if (canCastleKingSide &&
                    !IsKingSideCoveredByEnemy() &&
                    !IsAnyPieceOnKingSide())
                    availableMoves.Add("g" + row);
                if (canCastleQueenSide &&
                    !IsQueenSideCoveredByEnemy() &&
                    !IsAnyPieceOnQueenSide())
                    availableMoves.Add("c" + row);
            }

            ColorType enemyColor = GetOppositeColor(color);

            // Right
            string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, pieceCoordinates.Y + 1));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Down
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, pieceCoordinates.Y - 1));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Up Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y + 1));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Up Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y + 1));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Down Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y - 1));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Down Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y - 1));
            if (!BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, currentCoordinates))
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            return availableMoves;
        }

        public bool IsKingSideCoveredByEnemy()
        {
            string row = color == ColorType.White ? "1" : "8";
            ColorType enemyColor = GetOppositeColor(color);
            List<Piece> enemyPieces = chessBoard.GetPiecesByColor(enemyColor);

            foreach (var piece in enemyPieces)
            {
                if (piece is King)
                    continue;

                HashSet<string> availableMoves = GetAvailableMoves();
                if (availableMoves.Contains("f" + row) ||
                    availableMoves.Contains("g" + row))
                    return true;
            }

            return false;
        }

        public bool IsAnyPieceOnKingSide()
        {
            string row = color == ColorType.White ? "1" : "8";

            return
                chessBoard.GetPiece("f" + row) != null ||
                chessBoard.GetPiece("g" + row) != null;
        }

        public bool IsAnyPieceOnQueenSide()
        {
            string row = color == ColorType.White ? "1" : "8";

            return
                chessBoard.GetPiece("d" + row) != null ||
                chessBoard.GetPiece("c" + row) != null ||
                chessBoard.GetPiece("b" + row) != null;
        }

        public bool IsQueenSideCoveredByEnemy()
        {
            ColorType enemyColor = GetOppositeColor(color);
            List<Piece> enemyPieces = chessBoard.GetPiecesByColor(enemyColor);
            string row = color == ColorType.White ? "1" : "8";

            foreach (var piece in enemyPieces)
            {
                if (piece is King)
                    continue;

                HashSet<string> availableMoves = piece.GetAvailableMoves();
                if (availableMoves.Contains("d" + row) ||
                    availableMoves.Contains("c" + row))
                    return true;
            }

            return false;
        }

        public bool IsInCheck()
        {
            ColorType enemyColor = GetOppositeColor(color);
            return BoardHelper.IsSquareControlledByColor(chessBoard, enemyColor, coordinates);
        }
    }
}
