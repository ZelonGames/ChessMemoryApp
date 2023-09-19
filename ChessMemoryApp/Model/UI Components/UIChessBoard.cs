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

        public readonly SquareCollection squareCollection;
        public readonly PieceUICollection pieceUICollection;

        public Piece.Coordinates<double> offset = new();
        public Size BoardSize { get; private set; }

        public readonly Piece.ColorType boardColorOrientation;
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
            squareCollection = new SquareCollection(this, chessBoardListLayout);
            pieceUICollection = new PieceUICollection(this);
            boardColorOrientation = chessBoard.boardColorOrientation;
        }

        public void UpdateBoardSize(double size)
        {
            BoardSize = new Size(size, size);
        }

        public virtual void UpdateSquaresBounds()
        {
            foreach (var square in squareCollection.Squares)
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
            double chessBoardSize = squareCollection.chessBoardListLayout.Bounds.Height;
            columnChessBoard.Width = chessBoardSize;
            UpdateBoardSize(chessBoardSize);

            foreach (var square in squareCollection.Squares)
            {
                square.Value.contentView.WidthRequest = square.Value.contentView.HeightRequest = chessBoardSize / 8f;
                UpdateSquareBounds(square.Key, square.Value);
            }

            SizeChanged?.Invoke(chessBoardSize, squareCollection.chessBoardListLayout.Bounds);
        }

        public void Render()
        {
            RenderSquares();
            RenderPiecesFromChessBoard();
            Loaded?.Invoke(chessBoardData.GetPositionFen());
        }

        private void OnChangedPieces(MovedPieceData movedPieceData)
        {
            if (!squareCollection.HasSquares)
                return;

            foreach (var removedPiece in movedPieceData.removedPieces)
                pieceUICollection.RemovePiece(removedPiece.Key);

            foreach (var addedPiece in movedPieceData.addedPieces)
                pieceUICollection.AddPieceToSquare(chessBoardData.GetPiece(addedPiece.Key), squareCollection.GetSquare(addedPiece.Key));
        }

        private void RenderPiecesFromChessBoard()
        {
            pieceUICollection.ClearPieces();
            foreach (var piece in chessBoardData.pieces)
            {
                if (!pieceUICollection.PieceExists(piece.Value.coordinates))
                    pieceUICollection.AddPieceToSquare(piece.Value, squareCollection.GetSquare(piece.Key));
            }
        }

        public void ClearBoard()
        {
            pieceUICollection.ClearPieces();
            squareCollection.ClearSquares();
        }

        private void RenderSquares()
        {
            for (int row = 1; row <= 8; row++)
            {
                for (int column = 1; column <= 8; column++)
                {
                    Color color = (row + column) % 2 != 0 ? white : black;
                    squareCollection.AddChessBoardSquare(color, column, row);
                }
            }

            UpdateSquaresBounds();
        }
    }
}
