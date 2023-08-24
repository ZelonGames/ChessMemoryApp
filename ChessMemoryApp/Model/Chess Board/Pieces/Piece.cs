using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Piece
    {
        public static readonly Dictionary<char, string> pieceFileNames = new()
        {
            { 'p', "pawn_black" },
            { 'P', "pawn_white" },
            { 'r', "rook_black" },
            { 'R', "rook_white" },
            { 'n', "knight_black" },
            { 'N', "knight_white" },
            { 'b', "bishop_black" },
            { 'B', "bishop_white" },
            { 'q', "queen_black" },
            { 'Q', "queen_white" },
            { 'k', "king_black" },
            { 'K', "king_white" },
        };

        public enum ColorType
        {
            White,
            Black,
            Empty
        }

        public class Coordinates<T> where T : struct, IComparable, IComparable<T>, IEquatable<T>, IConvertible
        {
            public T X { get; set; }
            public T Y { get; set; }

            public Coordinates()
            {
            }
            public Coordinates(T x, T y)
            {
                X = x; Y = y;
            }
        }

        public HashSet<string> availableMoves = new();
        public Coordinates<int> currentCoordinate = new();
        public readonly Image image;
        public readonly char pieceChar;
        public readonly ColorType color;
        private readonly ChessboardGenerator chessBoard;

        public Piece(ChessboardGenerator chessBoard, ColorType color, char pieceChar)
        {
            this.chessBoard = chessBoard;
            this.color = color;

            this.pieceChar = color == ColorType.White ? char.ToUpper(pieceChar) : char.ToLower(pieceChar);
            pieceFileNames.TryGetValue(this.pieceChar, out string value);

            this.image = new Image()
            {
                Source = ImageSource.FromFile(value + ".png"),
            };
        }
        public Piece(ChessboardGenerator chessBoard, char pieceChar, bool isClickable = true)
        {
            this.chessBoard = chessBoard;
            this.pieceChar = pieceChar;

            pieceFileNames.TryGetValue(pieceChar, out string fileName);
            image = new Image()
            {
                Source = ImageSource.FromFile(fileName + ".png"),
            };

            if (isClickable)
                UIEventHelper.ImageClickSubscribe(this.image, OnPieceClicked);
        }

        public static ColorType GetColorOfPieceFromFen(string fen, string coordinates)
        {
            char? piece = FenHelper.GetPieceOnSquare(fen, coordinates);
            if (!piece.HasValue)
                return ColorType.Empty;

            return char.IsUpper(piece.Value) ? ColorType.White : ColorType.Black;
        }

        public void ResetEvents()
        {
            UIEventHelper.ImageClickUnSubscribe(image, OnPieceClicked);
            image.GestureRecognizers.Clear();
        }

        public void OnPieceClicked(object sender, EventArgs e)
        {
            if (chessBoard.MoveNotationHelper == null)
                return;

            bool clickedOnPiece = chessBoard.GetPiece(BoardHelper.GetLetterCoordinates(currentCoordinate)) != null;

            if (clickedOnPiece && chessBoard.MoveNotationHelper.IsFirstClick)
            {
                chessBoard.MoveNotationHelper.SetFirstClick(currentCoordinate);
                chessBoard.GetSquare(BoardHelper.GetLetterCoordinates(currentCoordinate)).HighlightSquare();
            }
            else
            {
                chessBoard.MoveNotationHelper.SetSecondClick(currentCoordinate);
                Square.HighlightedSquare?.LowlightSquare();
            }
        }

        public static ColorType GetColorFromChessboard(ChessboardGenerator chessBoard)
        {
            return chessBoard.playAsBlack ? ColorType.Black : ColorType.White;
        }

        public static ColorType GetOppositeColor(ColorType colorType)
        {
            return colorType == ColorType.White ? ColorType.Black : ColorType.White;
        }

        public static ColorType GetColorOfPiece(char piece)
        {
            return char.IsUpper(piece) ? ColorType.White : ColorType.Black;
        }

        public static bool IsPieceByColorOnSquare(string fen, string squareCoordinates, ColorType pieceColor)
        {
            char? pieceType = FenHelper.GetPieceOnSquare(fen, squareCoordinates);
            if (pieceType.HasValue)
            {
                // Uppercase = white | Lowercase = black
                bool isPieceSameAsGivenColor = 
                    pieceColor.Equals(ColorType.White) && char.IsUpper(pieceType.Value) ||
                    pieceColor.Equals(ColorType.Black) && char.IsLower(pieceType.Value);
                if (isPieceSameAsGivenColor)
                    return true;
            }

            return false;
        }

        public static bool AddVerticalMove(HashSet<string> availableMoves, string fen, Coordinates<int> currentCoordinate, int value, ColorType color)
        {
            string nextPositionNotation = GetNextPositionNotation(currentCoordinate.X, value);
            return TryAddMove(availableMoves, fen, nextPositionNotation);
        }

        public static bool AddHorizontalMove(HashSet<string> availableMoves, string fen, Coordinates<int> currentCoordinate, int value, ColorType color)
        {
            string nextPositionNotation = GetNextPositionNotation(value, currentCoordinate.Y);
            return TryAddMove(availableMoves, fen, nextPositionNotation);
        }

        public static bool AddDiagonalMove(HashSet<string> availableMoves, string fen, int x, int y, ColorType color)
        {
            string nextPositionNotation = GetNextPositionNotation(x, y);
            return TryAddMove(availableMoves, fen, nextPositionNotation);
        }


        public static bool TryAddMove(HashSet<string> availableMoves, string fen, string squareCoordinates)
        {
            return true;
        }

        /// <summary>
        /// Adds coordinates of a square that the chess piece can go to
        /// It fails to add a move if there is a friendly piece on that square
        /// It succeeds to add a move if there is no piece or an enemy piece on that square
        /// </summary>
        /// <param name="availableMoves">The list to add the squares to</param>
        /// <param name="fen">Used to know where the pieces are</param>
        /// <param name="pieceCoordinates">The square that the piece is on</param>
        /// <param name="squareCoordinates">The square to add a move on</param>
        /// <returns>returns false if there is a piece on the given 
        /// squareCoordinates according to the fen</returns>
        protected static bool TryAddMove(HashSet<string> availableMoves, string fen, string pieceCoordinates, string squareCoordinates)
        {
            if (squareCoordinates == "g4")
            {

            }

            char? piece = FenHelper.GetPieceOnSquare(fen, pieceCoordinates).Value;
            if (!piece.HasValue)
                return false;

            ColorType pieceColor = GetColorOfPiece(piece.Value);
            ColorType enemyPieceColor = GetOppositeColor(pieceColor);
            if (IsPieceByColorOnSquare(fen, squareCoordinates, enemyPieceColor))
            {
                availableMoves.Add(squareCoordinates);
                return false;
            }

            if (IsPieceByColorOnSquare(fen, squareCoordinates, pieceColor))
                return false;

            availableMoves.Add(squareCoordinates);
            return true;
        }

        private static string GetNextPositionNotation(int x, int y)
        {
            var nextPosition = new Coordinates<int>(x, y);
            return BoardHelper.GetLetterCoordinates(nextPosition);
        }
    }
}
