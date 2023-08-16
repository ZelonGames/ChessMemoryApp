using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'b')
        {

        }

        public static HashSet<string> GetAvailableMoves(Coordinates<int> currentCoordinate, Piece.ColorType color, string fen)
        {
            var availableMoves = new HashSet<string>();

            // Going up and right
            for (int x = currentCoordinate.X + 1, y = currentCoordinate.Y + 1; x <= 8 && y <= 8; x++, y++)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, color))
                    break;
            }

            // Going up and left
            for (int x = currentCoordinate.X - 1, y = currentCoordinate.Y + 1; x >= 1 && y <= 8; x--, y++)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, color))
                    break;
            }

            // Going down and right
            for (int x = currentCoordinate.X + 1, y = currentCoordinate.Y - 1; x <= 8 && y >= 1; x++, y--)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, color))
                    break;
            }

            // Going down and left
            for (int x = currentCoordinate.X - 1, y = currentCoordinate.Y - 1; x >= 1 && y >= 1; x--, y--)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, color))
                    break;
            }

            return availableMoves;
        }
    }
}
