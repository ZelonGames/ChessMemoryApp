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
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Right Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 2, pieceCoordinates.Y + 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Up Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y + 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Up Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y + 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Left Up
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 2, pieceCoordinates.Y + 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Left Down
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 2, pieceCoordinates.Y - 1));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Down Left
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X - 1, pieceCoordinates.Y - 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Down Right
            currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(pieceCoordinates.X + 1, pieceCoordinates.Y - 2));
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            return availableMoves;
        }
    }
}
