using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.UI_Helpers;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class ChessboardGenerator
    {
        public delegate void ChessBoardLoadedEventHandler(string fen);
        public event ChessBoardLoadedEventHandler Loaded;
        public delegate void MovedPieceEventHandler(string fromCoordinates, string toCoordinates);
        public event MovedPieceEventHandler MovedPiece;

        public delegate void SizeChangedEventHandler(double size, Rect bounds);
        public event SizeChangedEventHandler SizeChanged;

        public Dictionary<string, Piece> pieces = new();
        public readonly Dictionary<string, Square> squares = new();

        protected readonly AbsoluteLayout chessBoardListLayout;

        public FenSettings fenSettings = new();
        public Piece.Coordinates<double> offset = new();
        public Size BoardSize { get; private set; }

        public bool IsEmpty => pieces.Count == 0;
        public Piece.ColorType boardColorOrientation;

        public static readonly Color white = Color.FromArgb("#f0d9b5");
        public static readonly Color black = Color.FromArgb("#b58863");

        public readonly bool arePiecesAndSquaresClickable;
        private readonly ColumnDefinition columnChessBoard;

        public MoveNotationGenerator MoveNotationHelper { get; private set; }

        public ChessboardGenerator(AbsoluteLayout chessBoardListLayout, ColumnDefinition columnChessBoard, bool playAsBlack)
        {
            this.columnChessBoard = columnChessBoard;
            boardColorOrientation = playAsBlack ? Piece.ColorType.Black : Piece.ColorType.White;
            fenSettings.EnableAllCastleMoves();
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromPieceColor(Piece.GetOppositeColor(boardColorOrientation)));
            arePiecesAndSquaresClickable = true;
            this.chessBoardListLayout = chessBoardListLayout;
        }

        public ChessboardGenerator(AbsoluteLayout chessBoardListLayout, Size size, bool playAsBlack)
        {
            arePiecesAndSquaresClickable = false;
            this.chessBoardListLayout = chessBoardListLayout;
            BoardSize = size;

            boardColorOrientation = playAsBlack ? Piece.ColorType.Black : Piece.ColorType.White;
            fenSettings.EnableAllCastleMoves();
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromChessBoard(this));
        }

        public ChessboardGenerator()
        {
            arePiecesAndSquaresClickable = false;
        }

        public void SetMoveNotationHelper(MoveNotationGenerator moveNotationHelper)
        {
            this.MoveNotationHelper = moveNotationHelper;
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

        public void LoadChessBoardFromFen(string fen)
        {
            if (!FenHelper.IsValidFen(fen))
                return;

            if (squares.Count == 0)
                LoadSquares();

            LoadPiecesFromFen(fen);
            Loaded?.Invoke(fen);
        }

        public void LoadPiecesFromFen(string newFen)
        {
            var piecesToRemove = new List<Piece>();
            var piecesToAdd = new Dictionary<string, char>();

            Dictionary<string, char?> newPieces = FenHelper.GetPiecesFromFen(newFen);

            if (!IsEmpty)
            {
                foreach (var piece in pieces)
                {
                    bool isPieceCaptured =
                        newPieces[piece.Key].HasValue &&
                        newPieces[piece.Key].Value != piece.Value.pieceType;

                    bool isNewPieceSameAsCurrentPiece =
                        newPieces[piece.Key].HasValue &&
                        newPieces[piece.Key].Value == piece.Value.pieceType;

                    if ((!newPieces[piece.Key].HasValue || isPieceCaptured) &&
                        pieces.ContainsKey(piece.Key) && !isNewPieceSameAsCurrentPiece)
                        piecesToRemove.Add(piece.Value);
                }
            }

            foreach (var newPiece in newPieces)
            {
                bool isPieceCaptured =
                    pieces.ContainsKey(newPiece.Key) &&
                    newPiece.Value.HasValue &&
                    pieces[newPiece.Key].pieceType != newPiece.Value.Value;

                if (!pieces.ContainsKey(newPiece.Key) && newPiece.Value.HasValue ||
                    isPieceCaptured)
                    piecesToAdd.Add(newPiece.Key, newPiece.Value.Value);
            }

            foreach (var pieceToRemove in piecesToRemove)
                RemovePiece(pieceToRemove);

            foreach (var pieceToAdd in piecesToAdd)
                AddPieceToSquare(pieceToAdd.Value, pieceToAdd.Key);
        }

        public void ClearPieces()
        {
            foreach (var piece in pieces.Values)
                RemovePiece(piece);

            pieces.Clear();
        }

        public void RemovePiece(Piece piece)
        {
            piece.ResetEvents();
            UILogicHelper.RemovePieceFromSquare(this, piece);
            pieces.Remove(piece.coordinates);
        }

        public void ClearSquares()
        {
            foreach (var square in squares.Values)
            {
                square.contentView.Content = null;
                square.contentView.GestureRecognizers.Clear();
                chessBoardListLayout.Remove(square.contentView);
            }

            squares.Clear();
        }

        public void ClearBoard()
        {
            ClearPieces();
            ClearSquares();
        }

        public void LoadSquares()
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
            var square = new Square(contentViewSquare, coordinates, arePiecesAndSquaresClickable);
            if (MoveNotationHelper != null)
                square.SetMoveNotationGenerator(MoveNotationHelper);
            string letterCoordinates = square.coordinates;

            chessBoardListLayout.Add(square.contentView);
            squares.Add(letterCoordinates, square);

            return square;
        }

        public Square GetSquare(string coordinates)
        {
            return squares[coordinates];
        }

        public void MakeMove(string fromCoordinates, string toCoordinates)
        {
            Piece movedPiece = GetPiece(fromCoordinates);

            if (TryMakeCastleMove(fromCoordinates, toCoordinates))
            {
                MovedPiece?.Invoke(fromCoordinates, toCoordinates);
                return;
            }

            Piece capturedPiece = GetPiece(toCoordinates);
            if (movedPiece is Pawn)
            {
                string enPassantSquare = fenSettings.GetEnPassantSquare();
                if (enPassantSquare == toCoordinates)
                {
                    // The captured pawn is behind the moved piece after an en passant move
                    int captureDirection = movedPiece.color == Piece.ColorType.White ? -1 : 1;
                    Piece.Coordinates<int> numberCoordinates = BoardHelper.GetNumberCoordinates(toCoordinates);
                    numberCoordinates.Y += captureDirection;
                    string captureCoordinates = BoardHelper.GetLetterCoordinates(numberCoordinates);
                    capturedPiece = GetPiece(captureCoordinates);
                }
            }
            
            if (capturedPiece != null)
                RemovePiece(capturedPiece);

            RemovePiece(movedPiece);
            AddPieceToSquare(movedPiece.pieceType, toCoordinates);

            MovedPiece?.Invoke(fromCoordinates, toCoordinates);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromCoordinates"></param>
        /// <param name="toCoordinates"></param>
        /// <returns>returns true if a castle move was made</returns>
        private bool TryMakeCastleMove(string fromCoordinates, string toCoordinates)
        {
            Piece piece = GetPiece(fromCoordinates);
            if (piece == null || piece != null && piece.pieceType != 'k' && piece.pieceType != 'K')
                return false;

            string moveNotationCoordinates = fromCoordinates + toCoordinates;

            // Short Castle White
            Piece rook = GetPiece("h1");
            if (rook != null && rook.color == Piece.ColorType.White && moveNotationCoordinates == "e1g1")
            {
                MakeMove(fromCoordinates, toCoordinates);
                MakeMove("h1", "f1");
                return true;
            }

            // Long Castle White
            rook = GetPiece("a1");
            if (rook != null && rook.color == Piece.ColorType.White && moveNotationCoordinates == "e1c1")
            {
                MakeMove(fromCoordinates, toCoordinates);
                MakeMove("a1", "d1");
                return true;
            }

            // Short Castle Black
            rook = GetPiece("h8");
            if (rook != null && rook.color == Piece.ColorType.Black && moveNotationCoordinates == "e8g8")
            {
                MakeMove(fromCoordinates, toCoordinates);
                MakeMove("h8", "f8");
                return true;
            }

            // Long Castle Black
            rook = GetPiece("a8");
            if (rook != null && rook.color == Piece.ColorType.Black && moveNotationCoordinates == "e8c8")
            {
                MakeMove(fromCoordinates, toCoordinates);
                MakeMove("a8", "d8");
                return true;
            }

            return false;
        }

        public Piece AddPieceToSquare(char pieceType, string squareCoordinates)
        {
            bool pieceImageFileExists = Piece.pieceFileNames.TryGetValue(pieceType, out _);

            if (pieceImageFileExists)
            {
                string className = Piece.pieceNames[char.ToUpper(pieceType)];
                Type type = Type.GetType("ChessMemoryApp.Model.Chess_Board.Pieces." + className);
                Piece piece = (Piece)Activator.CreateInstance(type, this, pieceType);
                piece.coordinates = squareCoordinates;
                pieces.Add(piece.coordinates, piece);

                Square square = GetSquare(squareCoordinates);
                if (square != null)
                    square.contentView.Content = piece.image;

                return piece;
            }

            return null;
        }

        public Dictionary<string, Piece> GetPieceOfType(char pieceType)
        {
            var pieces = new Dictionary<string, Piece>();

            foreach (var piece in this.pieces)
            {
                if (piece.Value.pieceType == pieceType)
                    pieces.Add(piece.Key, piece.Value);
            }

            return pieces;
        }

        public Piece GetPiece(string coordinates)
        {
            pieces.TryGetValue(coordinates, out Piece piece);
            return piece;
        }

        public List<Piece> GetPiecesByColor(Piece.ColorType color)
        {
            var pieces = new List<Piece>();

            foreach (var piece in pieces)
            {
                if (piece.color == color)
                    pieces.Add(piece);
            }

            return pieces;
        }

        public string GetFen()
        {
            string fen = BoardHelper.GetPositionFenFromChessBoardPieces(pieces);
            fen += fenSettings.GetAppliedSettings(FenSettings.SpaceEncoding.SPACE);

            return fen;
        }
    }
}
