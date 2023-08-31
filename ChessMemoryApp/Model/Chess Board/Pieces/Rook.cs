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
        public Rook(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'r')
        {

        }

        public static HashSet<string> GetAvailableMoves(string pieceLetterCoordinates, string fen)
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(pieceLetterCoordinates);

            // Up
            for (int y = pieceCoordinates.Y + 1; y <= 8; y++)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            // Down
            for (int y = pieceCoordinates.Y - 1; y >= 1; y--)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X, y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            // Right
            for (int x = pieceCoordinates.X + 1; x <= 8; x++)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, pieceCoordinates.Y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            // Left
            for (int x = pieceCoordinates.X - 1; x >= 1; x--)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, pieceCoordinates.Y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            return availableMoves;
        }
    }
}
