using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.ChessMoveLogic;

namespace ChessMemoryApp.Model.UI_Components
{
    public class UIChessBoard
    {
        public delegate void ChessBoardLoadedEventHandler(string fen);
        public event ChessBoardLoadedEventHandler Loaded;
        public delegate void SizeChangedEventHandler(double size, Rect bounds);
        public event SizeChangedEventHandler SizeChanged;
        public event Action<PieceUI> ClickedPiece;
        public event Action<Square> ClickedSquare;

        private readonly AbsoluteLayout chessBoardListLayout;
        public readonly Dictionary<string, Square> squares = new();
        public readonly PieceUICollection pieceUICollection;

        public Piece.Coordinates<double> offset = new();
        public Size BoardSize { get; private set; }

        public Piece.ColorType boardColorOrientation;

        public static readonly Color white = Color.FromArgb("#f0d9b5");
        public static readonly Color black = Color.FromArgb("#b58863");

        public readonly bool isBoardClickable;
        private readonly ColumnDefinition columnChessBoard;
        public readonly ChessboardGenerator chessBoardData;

        public UIChessBoard(ChessboardGenerator chessBoard, AbsoluteLayout chessBoardListLayout, ColumnDefinition columnChessBoard) :
            this(chessBoard, chessBoardListLayout)
        {
            this.columnChessBoard = columnChessBoard;
            isBoardClickable = true;
            chessBoard.ChangedPieces += OnChangedPieces;
        }

        public UIChessBoard(ChessboardGenerator chessBoard, AbsoluteLayout chessBoardListLayout, Size size) :
            this(chessBoard, chessBoardListLayout)
        {
            isBoardClickable = false;
            BoardSize = size;
        }

        private UIChessBoard(ChessboardGenerator chessBoard, AbsoluteLayout chessBoardListLayout)
        {
            this.chessBoardData = chessBoard;
            this.chessBoardListLayout = chessBoardListLayout;
            pieceUICollection = new PieceUICollection(this);
            boardColorOrientation = chessBoard.boardColorOrientation;
        }

        public UIChessBoard(ChessboardGenerator chessBoard)
        {
            this.chessBoardData = chessBoard;
            isBoardClickable = false;
        }

        public void UpdateBoardSize(double size)
        {
            BoardSize = new Size(size, size);
        }

        public virtual void UpdateSquaresBounds()
        {
            foreach (var square in squares)
                UpdateSquareBounds(square.Key, square.Value);
        }

        private void UpdateSquareBounds(string letterCoordinates, Square square)
        {
            Piece.Coordinates<int> coordinates = BoardHelper.GetNumberCoordinates(letterCoordinates);

            int xCoordinates = coordinates.X - 1;
            int yCoordinates = 8 - coordinates.Y;

            if (boardColorOrientation == Piece.ColorType.Black)
            {
                xCoordinates = 8 - coordinates.X;
                yCoordinates = coordinates.Y - 1;
            }

            square.contentView.WidthRequest = square.contentView.HeightRequest = BoardSize.Width / 8d;
            square.contentView.TranslationX = offset.X + xCoordinates * square.contentView.WidthRequest;
            square.contentView.TranslationY = offset.Y + yCoordinates * square.contentView.HeightRequest;
        }

        public void UpdateSquaresViewSize(object sender, EventArgs e)
        {
            double chessBoardSize = chessBoardListLayout.Bounds.Height;
            columnChessBoard.Width = chessBoardSize;
            UpdateBoardSize(chessBoardSize);

            foreach (var square in squares)
            {
                square.Value.contentView.WidthRequest = square.Value.contentView.HeightRequest = chessBoardSize / 8f;
                UpdateSquareBounds(square.Key, square.Value);
            }

            SizeChanged?.Invoke(chessBoardSize, chessBoardListLayout.Bounds);
        }

        public void Render()
        {
            RenderSquares();
            RenderPiecesFromChessBoard();
            Loaded?.Invoke(chessBoardData.GetPositionFen());
        }

        private void OnChangedPieces(MovedPieceData movedPieceData)
        {
            if (squares.Count == 0)
                return;

            foreach (var removedPiece in movedPieceData.removedPieces)
            {
                pieceUICollection.GetPiece(removedPiece.Key).Clicked -= OnClickedPiece;
                pieceUICollection.RemovePiece(pieceUICollection.GetPiece(removedPiece.Key));
            }

            foreach (var addedPiece in movedPieceData.addedPieces)
            {
                pieceUICollection.AddPieceToSquare(chessBoardData.GetPiece(addedPiece.Key), squares[addedPiece.Key]);
                pieceUICollection.GetPiece(addedPiece.Key).Clicked += OnClickedPiece;
            }
        }

        private void OnClickedPiece(PieceUI clickedPiece)
        {
            ClickedPiece?.Invoke(clickedPiece);
        }

        private void OnClickedSquare(Square clickedSquare)
        {
            ClickedSquare?.Invoke(clickedSquare);
        }

        private void RenderPiecesFromChessBoard()
        {
            pieceUICollection.ClearPieces();
            foreach (var piece in chessBoardData.pieces)
            {
                if (!pieceUICollection.PieceExists(piece.Value.coordinates))
                {
                    pieceUICollection.AddPieceToSquare(piece.Value, squares[piece.Key]);
                    pieceUICollection.GetPiece(piece.Value.coordinates).Clicked += OnClickedPiece;
                }
            }
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

        public void ClearBoard()
        {
            pieceUICollection.ClearPieces();
            ClearSquares();
        }

        private void RenderSquares()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    int newColumn = column + 1;
                    int newRow = row + 1;

                    Color color = (newRow + newColumn) % 2 != 0 ? white : black;
                    AddChessBoardSquare(color, newColumn, newRow);
                }
            }

            UpdateSquaresBounds();
        }

        private Square AddChessBoardSquare(Color color, int column, int row)
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
            var square = new Square(contentViewSquare, coordinates, isBoardClickable);
            string letterCoordinates = square.coordinates;

            chessBoardListLayout.Add(square.contentView);
            square.Clicked += OnClickedSquare;
            squares.Add(letterCoordinates, square);

            return square;
        }
    }
}
