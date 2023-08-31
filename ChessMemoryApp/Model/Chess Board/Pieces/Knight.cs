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
        public Knight(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'n')
        {

        }

        public static HashSet<string> GetAvailableMoves(string pieceLetterCoordinates, string fen)
        {
            var availableMoves = new HashSet<string>();

            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(pieceLetterCoordinates);

            // Right Down
            string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 2, pieceCoordinates.Y - 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Right Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 2, pieceCoordinates.Y + 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Up Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y + 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Up Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y + 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Left Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 2, pieceCoordinates.Y + 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Left Down
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 2, pieceCoordinates.Y - 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Down Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y - 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            // Down Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y - 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

            return availableMoves;
        }
    }
}
