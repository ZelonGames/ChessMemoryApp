using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'p')
        {

        }
        public static HashSet<string> GetAvailableMoves(Coordinates<int> currentCoordinate, Piece.ColorType color, string fen, bool whiteDirection, bool isCapture)
        {
            var availableMoves = new HashSet<string>();
            string nextPositionNotation = "";

            var nextPosition = GetNextPosition(whiteDirection, currentCoordinate, new Coordinates<int>(0, 1));
            if (!isCapture)
            {
                // Forward
                nextPositionNotation = BoardHelper.GetLetterCoordinates(nextPosition);
                if (nextPositionNotation != "" && !IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color))
                {
                    availableMoves.Add(nextPositionNotation);

                    nextPosition = GetNextPosition(whiteDirection, currentCoordinate, new Coordinates<int>(0, 2));
                    nextPositionNotation = BoardHelper.GetLetterCoordinates(nextPosition);

                    if (nextPositionNotation != "" && !IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color)) availableMoves.Add(nextPositionNotation);
                }
            }
            else
            {
                // Diagonal Right Capture
                nextPosition = GetNextPosition(whiteDirection, currentCoordinate, new Coordinates<int>(1, 1));
                nextPositionNotation = BoardHelper.GetLetterCoordinates(nextPosition);
                if (nextPositionNotation != "" && !IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color))
                    availableMoves.Add(BoardHelper.GetLetterCoordinates(nextPosition));

                // Diagonal Left Capture
                nextPosition = GetNextPosition(whiteDirection, currentCoordinate, new Coordinates<int>(-1, 1));
                nextPositionNotation = BoardHelper.GetLetterCoordinates(nextPosition);
                if (nextPositionNotation != "" && !IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color))
                    availableMoves.Add(BoardHelper.GetLetterCoordinates(nextPosition));
            }

            return availableMoves;
        }

        private static Coordinates<int> GetNextPosition(bool whiteDirection, Coordinates<int> currentCoordinate, Coordinates<int> steps)
        {
            return new Coordinates<int>(currentCoordinate.X + steps.X, currentCoordinate.Y + (whiteDirection ? steps.Y : steps.Y * -1));
        }
    }
}
