using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Helpers;
using System.Data.Common;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using Microsoft.Maui.Controls;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.ChessMoveLogic;
using static SQLite.TableMapping;

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

        public void LoadFen(string fen)
        {
            if (!FenHelper.IsValidFen(fen))
                return;

            currentFen = fen;
            LoadTemporaryFen(fen);
        }

        public void LoadTemporaryFen(string fen)
        {
            if (!FenHelper.IsValidFen(fen))
                return;

            ClearPieces();
            LoadPieces(fen);

            Loaded?.Invoke(fen);
        }

        public void LoadPieces(string fen)
        {
            foreach (var square in squares)
            {
                Piece.Coordinates<int> coordinates = BoardHelper.GetNumberCoordinates(square.Key);
                TryAddPieceFromFEN(square.Value, fen);
            }
        }

        public void ClearPieces()
        {
            foreach (var piece in pieces.Values)
            {
                piece.ResetEvents();
                UILogicHelper.RemovePieceFromSquare(this, piece);
            }

            pieces.Clear();
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

        public void LoadChessBoard()
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

        /// <summary>
        /// Tries to add a piece to a given square based on FEN
        /// </summary>
        /// <param name="square">If this square is in the FEN, a piece will be added to it</param>
        /// <param name="fen"></param>
        /// <param name="tempPieces"></param>
        /// <returns></returns>
        private Piece TryAddPieceFromFEN(Square square, string fen)
        {
            Piece piece = null;

            char? pieceChar = FenHelper.GetPieceOnSquare(fen, BoardHelper.GetLetterCoordinates(
                new Piece.Coordinates<int>(square.coordinate.X, square.coordinate.Y)));

            if (!pieceChar.HasValue)
                return null;

            bool pieceImageFileExists = Piece.pieceFileNames.TryGetValue(pieceChar.Value, out string fileName);

            if (pieceImageFileExists)
            {
                piece = new Piece(this, pieceChar.Value, ImageSource.FromFile(fileName + ".png"), arePiecesAndSquaresClickable);
                piece.currentCoordinate = new Piece.Coordinates<int>(square.coordinate.X, square.coordinate.Y);

                pieces.Add(BoardHelper.GetLetterCoordinates(piece.currentCoordinate), piece);
                square.contentView.Content = piece.image;
            }

            return piece;
        }

        public Square GetSquare(string coordinates)
        {
            return squares[coordinates];
        }

        public Piece GetPiece(string coordinates)
        {
            pieces.TryGetValue(coordinates, out Piece piece);
            return piece;
        }
    }
}
