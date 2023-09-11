using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Rook : Piece
    {
        public Rook(ChessboardGenerator chessBoard, char pieceType) : base(chessBoard, pieceType)
        {

        }

        public override HashSet<string> GetAvailableMoves()
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(coordinates);

            // Up
            for (int y = pieceCoordinates.Y + 1; y <= 8; y++)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, y));
                if (TryAddMove(availableMoves, chessBoard, currentCoordinates).isPieceOnSquare)
                    break;
            }

            // Down
            for (int y = pieceCoordinates.Y - 1; y >= 1; y--)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, y));
                if (TryAddMove(availableMoves, chessBoard, currentCoordinates).isPieceOnSquare)
                    break;
            }

            // Right
            for (int x = pieceCoordinates.X + 1; x <= 8; x++)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, pieceCoordinates.Y));
                if (TryAddMove(availableMoves, chessBoard, currentCoordinates).isPieceOnSquare)
                    break;
            }

            // Left
            for (int x = pieceCoordinates.X - 1; x >= 1; x--)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, pieceCoordinates.Y));
                if (TryAddMove(availableMoves, chessBoard, currentCoordinates).isPieceOnSquare)
                    break;
            }

            return availableMoves;
        }
    }
}
