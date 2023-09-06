using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            White = 1,
            Black = 2,
            Empty = 0,
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
        public string currentCoordinates;
        public readonly Image image;
        public readonly char pieceType;
        public readonly ColorType color;
        private readonly ChessboardGenerator chessBoard;

        public Piece(ChessboardGenerator chessBoard, ColorType color, char pieceChar)
        {
            this.chessBoard = chessBoard;
            this.color = color;

            this.pieceType = color == ColorType.White ? char.ToUpper(pieceChar) : char.ToLower(pieceChar);
            pieceFileNames.TryGetValue(this.pieceType, out string value);

            this.image = new Image()
            {
                Source = ImageSource.FromFile(value + ".png"),
            };
        }
        public Piece(ChessboardGenerator chessBoard, char pieceChar, bool isClickable = true)
        {
            this.chessBoard = chessBoard;
            this.pieceType = pieceChar;

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

            bool clickedOnPiece = chessBoard.GetPiece((currentCoordinates)) != null;

            if (clickedOnPiece && chessBoard.MoveNotationHelper.IsFirstClick)
            {
                chessBoard.MoveNotationHelper.SetFirstClick(BoardHelper.GetNumberCoordinates(currentCoordinates));
                chessBoard.GetSquare(currentCoordinates).HighlightSquare();
            }
            else
            {
                chessBoard.MoveNotationHelper.SetSecondClick(BoardHelper.GetNumberCoordinates(currentCoordinates));
                Square.HighlightedSquare?.LowlightSquare();
            }
        }

        public static ColorType GetColorFromChessboard(ChessboardGenerator chessBoard)
        {
            return chessBoard.colorToPlay;
        }

        public static ColorType GetOppositeColor(ColorType colorType)
        {
            return colorType == ColorType.White ? ColorType.Black : ColorType.White;
        }

        public static ColorType GetColorOfPiece(char piece)
        {
            return char.IsUpper(piece) ? ColorType.White : ColorType.Black;
        }

        public virtual void GetAvailableMoves()
        {

        }

        public static HashSet<string> GetAvailableMoves(char pieceType, string pieceLetterCoordinates, string fen)
        {
            HashSet<string> availableMoves;

            switch (char.ToLower(pieceType))
            {
                case 'r':
                    availableMoves = Rook.GetAvailableMoves(pieceLetterCoordinates, fen);
                    break;
                case 'n':
                    availableMoves = Knight.GetAvailableMoves(pieceLetterCoordinates, fen);
                    break;
                case 'b':
                    availableMoves = Bishop.GetAvailableMoves(pieceLetterCoordinates, fen);
                    break;
                case 'q':
                    availableMoves = Queen.GetAvailableMoves(pieceLetterCoordinates, fen);
                    break;
                case 'k':
                    availableMoves = King.GetAvailableMoves(pieceLetterCoordinates, fen);
                    break;
                default:
                    availableMoves = Pawn.GetAvailableMoves(pieceLetterCoordinates, fen);
                    break;
            }

            return availableMoves;
        }

        public static bool IsPieceByColorOnSquare(string fen, string squareCoordinates, ColorType pieceColor, out char? pieceType)
        {
            pieceType = FenHelper.GetPieceOnSquare(fen, squareCoordinates);
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

        /// <summary>
        /// Adds a move that a piece can go to if it's a valid move.
        /// </summary>
        /// <param name="availableMoves">The list of moves the piece can go to</param>
        /// <param name="fen">The state of the board</param>
        /// <param name="pieceCoordinates">The position of the piece you are adding moves to</param>
        /// <param name="squareCoordinates">The move you want to try to add to the piece</param>
        /// <returns></returns>
        protected static TryAddMoveResult TryAddMove(HashSet<string> availableMoves, string fen, string pieceCoordinates, string squareCoordinates)
        {
            if (string.IsNullOrEmpty(squareCoordinates) || string.IsNullOrEmpty(pieceCoordinates))
            {
                return new TryAddMoveResult()
                {
                    isPieceOnSquare = false,
                    success = false,
                };
            }

            char? piece = FenHelper.GetPieceOnSquare(fen, pieceCoordinates);
            // Don't do anything if there is no piece to move
            if (!piece.HasValue)
            {
                return new TryAddMoveResult()
                {
                    isPieceOnSquare = false,
                    success = false,
                };
            }

            ColorType pieceColor = GetColorOfPiece(piece.Value);
            ColorType enemyPieceColor = GetOppositeColor(pieceColor);

            bool isPieceOnSquare = FenHelper.GetPieceOnSquare(fen, squareCoordinates).HasValue;
            bool isEnemyPieceOnSquare = IsPieceByColorOnSquare(fen, squareCoordinates, enemyPieceColor, out _);

            if (isEnemyPieceOnSquare)
            {
                availableMoves.Add(squareCoordinates);
                return new TryAddMoveResult()
                {
                    isPieceOnSquare = isPieceOnSquare,
                    success = true,
                };
            }
            else if (!isPieceOnSquare)
            {
                availableMoves.Add(squareCoordinates);
                return new TryAddMoveResult()
                {
                    isPieceOnSquare = isPieceOnSquare,
                    success = true,
                };
            }

            // A frierndly piece is on this square
            return new TryAddMoveResult()
            {
                isPieceOnSquare = isPieceOnSquare,
                success = false,
            };
        }

        protected struct TryAddMoveResult
        {
            public bool success;
            public bool isPieceOnSquare;
        }
    }
}
