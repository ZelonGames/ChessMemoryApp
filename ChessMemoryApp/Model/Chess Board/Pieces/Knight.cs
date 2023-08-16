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

        public static HashSet<string> GetAvailableMoves(Coordinates<int> currentCoordinate, ColorType color, string fen)
        {
            var availableMoves = new HashSet<string>();

            var nextPosition = new Coordinates<int>();
            string nextPositionNotation = "";

            int[] xMoves = new int[] { 2, 2, -2, -2, 1, 1, -1, -1 };
            int[] yMoves = new int[] { 1, -1, 1, -1, 2, -2, 2, -2 };

            for (int i = 0; i < xMoves.Length; i++)
            {
                int newX = currentCoordinate.X + xMoves[i];
                int newY = currentCoordinate.Y + yMoves[i];

                if (newX <= 0 || newX > 8 || newY <= 0 || newY > 8)
                    continue;

                nextPosition = new Coordinates<int>(newX, newY);
                nextPositionNotation = BoardHelper.GetLetterCoordinates(nextPosition);

                if (IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color))
                    continue;

                availableMoves.Add(nextPositionNotation);
            }

            return availableMoves;
        }
    }
}
