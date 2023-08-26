using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Helpers;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.ChessMoveLogic;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class ChessboardGenerator
    {
        public delegate void ChessBoardLoadedEventHandler(string fen);
        public event ChessBoardLoadedEventHandler Loaded;

        public delegate void SizeChangedEventHandler(double size, Rect bounds);
        public event SizeChangedEventHandler SizeChanged;

        private readonly Dictionary<string, Piece> pieces = new();
        public readonly Dictionary<string, Square> squares = new();

        protected readonly AbsoluteLayout chessBoardListLayout;

        public FenSettings fenSettings = new();
        public Piece.Coordinates<double> offset = new();
        public Size BoardSize { get; private set; }

        public bool IsEmpty => pieces.Count == 0;
        public bool playAsBlack;

        public string currentFen;
        public static readonly Color white = Color.FromArgb("#f0d9b5");
        public static readonly Color black = Color.FromArgb("#b58863");

        protected readonly bool arePiecesAndSquaresClickable;
        private readonly ColumnDefinition columnChessBoard;

        public MoveNotationGenerator MoveNotationHelper { get; private set; }

        public ChessboardGenerator(AbsoluteLayout chessBoardListLayout, ColumnDefinition columnChessBoard, bool playAsBlack)
        {
            this.columnChessBoard = columnChessBoard;
            this.playAsBlack = playAsBlack;
            fenSettings.EnableAllCastleMoves();
            fenSettings.SetColorToPlay(playAsBlack ? FenSettings.FenColor.WHITE : FenSettings.FenColor.BLACK);
            arePiecesAndSquaresClickable = true;
            this.chessBoardListLayout = chessBoardListLayout;

        }

        public ChessboardGenerator(AbsoluteLayout chessBoardListLayout, Size size)
        {
            arePiecesAndSquaresClickable = false;
            this.chessBoardListLayout = chessBoardListLayout;
            BoardSize = size;
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

            if (playAsBlack)
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

            LoadTemporaryFen(fen);

            currentFen = fen;

            if (squares.Count == 0)
                LoadSquares();
        }

        public void LoadTemporaryFen(string fen)
        {
            if (!FenHelper.IsValidFen(fen))
                return;

            if (squares.Count == 0)
                LoadSquares();

            if (currentFen == null)
                LoadPieces(fen, fen);
            else
                LoadPieces(currentFen, fen);

            Loaded?.Invoke(fen);
        }

        public void LoadPieces(string oldFen, string newFen)
        {
            if (oldFen == newFen && pieces.Count > 0)
                return;

            var piecesToRemove = new List<Piece>();
            var piecesToAdd = new Dictionary<string, char>();

            Dictionary<string, char?> newPieces = FenHelper.GetPiecesFromFen(newFen);
            Dictionary<string, char?> oldPieces = FenHelper.GetPiecesFromFen(oldFen);

            if (!IsEmpty)
            {
                foreach (var piece in oldPieces)
                {
                    if (!piece.Value.HasValue)
                        continue;

                    bool isPieceCaptured =
                        newPieces[piece.Key].HasValue && piece.Value.HasValue &&
                        newPieces[piece.Key].Value != piece.Value.Value;

                    bool isNewPieceSameAsCurrentPiece = 
                        newPieces[piece.Key].HasValue && 
                        newPieces[piece.Key].Value == pieces[piece.Key].pieceChar;

                    if ((!newPieces[piece.Key].HasValue || isPieceCaptured) && 
                        pieces.ContainsKey(piece.Key) && !isNewPieceSameAsCurrentPiece)
                        piecesToRemove.Add(pieces[piece.Key]);
                }
            }

            foreach (var newPiece in newPieces)
            {
                bool isPieceCaptured =
                    pieces.ContainsKey(newPiece.Key) &&
                    newPiece.Value.HasValue &&
                    pieces[newPiece.Key].pieceChar != newPiece.Value.Value;

                if (!pieces.ContainsKey(newPiece.Key) && newPiece.Value.HasValue ||
                    isPieceCaptured)
                    piecesToAdd.Add(newPiece.Key, newPiece.Value.Value);
            }

            foreach (var pieceToRemove in piecesToRemove)
                RemovePiece(pieceToRemove);

            foreach (var pieceToAdd in piecesToAdd)
            {
                Square square = squares[pieceToAdd.Key];
                AddPieceToSquare(pieceToAdd.Value, square);
            }
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
            string coordinates = BoardHelper.GetLetterCoordinates(piece.currentCoordinate);
            pieces.Remove(coordinates);
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

            var square = new Square(contentViewSquare, new Piece.Coordinates<int>(column, row), arePiecesAndSquaresClickable);
            if (MoveNotationHelper != null)
                square.SetMoveNotationGenerator(MoveNotationHelper);
            square.coordinate = new Piece.Coordinates<int>(column, row);
            string letterCoordinates = BoardHelper.GetLetterCoordinates(square.coordinate);

            chessBoardListLayout.Add(square.contentView);
            squares.Add(letterCoordinates, square);

            return square;
        }

        public Square GetSquare(string coordinates)
        {
            return squares[coordinates];
        }

        public Piece AddPieceToSquare(char pieceChar, Square square)
        {
            bool pieceImageFileExists = Piece.pieceFileNames.TryGetValue(pieceChar, out string fileName);

            if (pieceImageFileExists)
            {
                var piece = new Piece(this, pieceChar, arePiecesAndSquaresClickable);
                piece.currentCoordinate = new Piece.Coordinates<int>(square.coordinate.X, square.coordinate.Y);

                pieces.Add(BoardHelper.GetLetterCoordinates(piece.currentCoordinate), piece);
                square.contentView.Content = piece.image;

                return piece;
            }

            return null;
        }

        public Piece GetPiece(string coordinates)
        {
            pieces.TryGetValue(coordinates, out Piece piece);
            return piece;
        }
    }
}
