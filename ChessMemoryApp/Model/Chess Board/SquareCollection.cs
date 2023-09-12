using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class SquareCollection
    {
        private readonly UIChessBoard chessBoard;
        public readonly Dictionary<string, Square> squares = new();

        public SquareCollection(UIChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
        /*
        public Square AddChessBoardSquare(Color color, int column, int row)
        {
            double squareSize = BoardSize.Width / 8;

            // Create a new box view for the current square
            var contentViewSquare = new ContentView()
            {
                BackgroundColor = color,
                WidthRequest = squareSize,
                HeightRequest = squareSize,
                TranslationX = offset.X + column * squareSize,
                TranslationY = offset.Y + (row - 1) * squareSize,
            };

            
            string coordinates = BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(column, row));
            var square = new Square(contentViewSquare, coordinates, chessBoard.isBoardClickable);
            string letterCoordinates = square.coordinates;

            chessBoardListLayout.Add(square.contentView);
            square.Clicked += Square_Clicked;
            squares.Add(letterCoordinates, square);

            return square;
        }
        */
    }
}
