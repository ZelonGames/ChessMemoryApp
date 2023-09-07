using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Knight : Piece
    {
        public Knight(ChessboardGenerator chessBoard, char pieceType) : base(chessBoard, pieceType)
        {

        }

        public override HashSet<string> GetAvailableMoves()
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(coordinates);

            // Right Down
            string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 2, pieceCoordinates.Y - 1));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Right Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 2, pieceCoordinates.Y + 1));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Up Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y + 2));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Up Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y + 2));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Left Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 2, pieceCoordinates.Y + 1));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Left Down
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 2, pieceCoordinates.Y - 1));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Down Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y - 2));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Down Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y - 2));
            TryAddMove(availableMoves, chessBoard, currentCoordinates);

            return availableMoves;
        }
    }
}
