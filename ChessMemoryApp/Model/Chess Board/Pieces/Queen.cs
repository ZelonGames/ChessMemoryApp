using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Queen : Piece
    {
        public Queen(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'q')
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

            // Going up and right
            for (int x = currentCoordinate.X + 1, y = currentCoordinate.Y + 1; x <= 8 && y <= 8; x++, y++)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, ColorType))
                    break;
            }

            // Going up and left
            for (int x = currentCoordinate.X - 1, y = currentCoordinate.Y + 1; x >= 1 && y <= 8; x--, y++)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, ColorType))
                    break;
            }

            // Going down and right
            for (int x = currentCoordinate.X + 1, y = currentCoordinate.Y - 1; x <= 8 && y >= 1; x++, y--)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, ColorType))
                    break;
            }

            // Going down and left
            for (int x = currentCoordinate.X - 1, y = currentCoordinate.Y - 1; x >= 1 && y >= 1; x--, y--)
            {
                if (AddDiagonalMove(availableMoves, fen, x, y, ColorType))
                    break;
            }

            return availableMoves;
        }
    }
}
