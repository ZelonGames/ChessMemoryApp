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

        public static HashSet<string> GetAvailableMoves(Coordinates<int> currentCoordinate, ColorType ColorType, string fen)
        {
            var availableMoves = new HashSet<string>();

            // Up
            for (int y = currentCoordinate.Y + 1; y <= 8; y++)
            {
                if (AddVerticalMove(availableMoves, fen, currentCoordinate, y, ColorType))
                    break;
            }

            // Down
            for (int y = currentCoordinate.Y - 1; y >= 1; y--)
            {
                if (AddVerticalMove(availableMoves, fen, currentCoordinate, y, ColorType))
                    break;
            }

            // Right
            for (int x = currentCoordinate.X + 1; x <= 8; x++)
            {
                if (AddHorizontalMove(availableMoves, fen, currentCoordinate, x, ColorType))
                    break;
            }

            // Left
            for (int x = currentCoordinate.X - 1; x >= 1; x--)
            {
                if (AddHorizontalMove(availableMoves, fen, currentCoordinate, x, ColorType))
                    break;
            }

            return availableMoves;
        }
    }
}
