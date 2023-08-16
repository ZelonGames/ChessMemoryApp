using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class King : Piece
    {
        public King(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'k')
        {

        }

        public static HashSet<string> GetAvailableMoves(Coordinates<int> currentCoordinate, ColorType color, string fen)
        {
            var availableMoves = new HashSet<string>();

            var nextPosition = new Coordinates<int>();
            string nextPositionNotation = "";

            int[][] possibleDirections = new int[][] {
                new int[] { 1, 0 },
                new int[] { -1, 0 },
                new int[] { 0, 1 },
                new int[] { 0, -1 },
                new int[] { 1, 1 },
                new int[] { 1, -1 },
                new int[] { -1, 1 },
                new int[] { -1, -1 }
            };

            foreach (int[] direction in possibleDirections)
            {
                nextPosition = new Coordinates<int>(currentCoordinate.X + direction[0], currentCoordinate.Y + direction[1]);
                nextPositionNotation = BoardHelper.GetLetterCoordinates(nextPosition);

                if (nextPosition.X >= 1 && nextPosition.X <= 8 && nextPosition.Y >= 1 && nextPosition.Y <= 8)
                {
                    if (IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color))
                        continue;

                    availableMoves.Add(nextPositionNotation);
                }
            }

            return availableMoves;
        }
    }
}
