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
        public event Action<Square> ClickedSquare;

        private readonly UIChessBoard chessBoard;
        private readonly Dictionary<string, Square> squares = new();
        public Dictionary<string, Square> Squares => squares;
        public readonly AbsoluteLayout chessBoardListLayout;
        public bool HasSquares => squares.Count > 0;

        public SquareCollection(UIChessBoard chessBoard, AbsoluteLayout chessBoardListLayout)
        {
            this.chessBoard = chessBoard;
            this.chessBoardListLayout = chessBoardListLayout;
        }

        public Square GetSquare(string coordinates)
        {
            return squares[coordinates];
        }
        
        public Square AddChessBoardSquare(Color color, int column, int row)
        {
            double squareSize = chessBoard.BoardSize.Width / 8;

            // Create a new box view for the current square
            var contentViewSquare = new ContentView()
            {
                BackgroundColor = color,
                WidthRequest = squareSize,
                HeightRequest = squareSize,
                TranslationX = chessBoard.offset.X + column * squareSize,
                TranslationY = chessBoard.offset.Y + (row - 1) * squareSize,
            };
            
            string coordinates = BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(column, row));
            var square = new Square(contentViewSquare, coordinates, chessBoard.isBoardClickable);
            string letterCoordinates = square.coordinates;

            chessBoardListLayout.Add(square.contentView);
            square.Clicked += OnClickedSquare;
            squares.Add(letterCoordinates, square);

            return square;
        }

        public void ClearSquares()
        {
            foreach (var square in squares.Values)
            {
                square.Clicked -= OnClickedSquare;
                square.contentView.Content = null;
                square.contentView.GestureRecognizers.Clear();
                chessBoardListLayout.Remove(square.contentView);
            }

            squares.Clear();
        }

        private void OnClickedSquare(Square clickedSquare)
        {
            ClickedSquare?.Invoke(clickedSquare);
        }
    }
}
