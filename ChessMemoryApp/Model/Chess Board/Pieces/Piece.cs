using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
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
        public Piece(ChessboardGenerator chessBoard, char pieceChar, ImageSource source, bool isClickable = true)
        {
            this.chessBoard = chessBoard;
            this.pieceChar = pieceChar;
            image = new Image()
            {
                Source = source,
            };

            if (isClickable)
                UIEventHelper.ImageClickSubscribe(this.image, OnPieceClicked);
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

        public static bool IsFriendlyPieceOnSquare(HashSet<string> availableMoves, string fen, string nextPositionNotation, ColorType color)
        {
            char? pieceType = CourseMaker.FenHelper.GetPieceOnSquare(fen, nextPositionNotation);
            if (pieceType.HasValue)
            {
                if (color.Equals(ColorType.White) && char.IsUpper(pieceType.Value) ||
                    color.Equals(ColorType.Black) && char.IsLower(pieceType.Value))
                    availableMoves.Add(nextPositionNotation);
                return true;
            }

            return false;
        }

        public static bool AddVerticalMove(HashSet<string> availableMoves, string fen, Coordinates<int> currentCoordinate, int value, ColorType color)
        {
            string nextPositionNotation = GetNextPositionNotation(currentCoordinate.X, value);
            return AddMove(availableMoves, fen, nextPositionNotation, color);
        }

        public static bool AddHorizontalMove(HashSet<string> availableMoves, string fen, Coordinates<int> currentCoordinate, int value, ColorType color)
        {
            string nextPositionNotation = GetNextPositionNotation(value, currentCoordinate.Y);
            return AddMove(availableMoves, fen, nextPositionNotation, color);
        }

        public static bool AddDiagonalMove(HashSet<string> availableMoves, string fen, int x, int y, ColorType color)
        {
            string nextPositionNotation = GetNextPositionNotation(x, y);
            return AddMove(availableMoves, fen, nextPositionNotation, color);
        }

        private static bool AddMove(HashSet<string> availableMoves, string fen, string nextPositionNotation, ColorType color)
        {
            if (IsFriendlyPieceOnSquare(availableMoves, fen, nextPositionNotation, color))
                return true;

            availableMoves.Add(nextPositionNotation);
            return false;
        }

        private static string GetNextPositionNotation(int x, int y)
        {
            var nextPosition = new Coordinates<int>(x, y);
            return BoardHelper.GetLetterCoordinates(nextPosition);
        }
    }
}
