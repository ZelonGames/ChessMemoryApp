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
    public abstract class Piece
    {
        public static readonly Dictionary<char, string> pieceNames = new()
        {
            { nameof(Pawn)[0], nameof(Pawn) },
            { nameof(Bishop)[0], nameof(Bishop) },
            { char.ToUpper(nameof(Knight)[1]), nameof(Knight) },
            { nameof(Rook)[0], nameof(Rook) },
            { nameof(Queen)[0], nameof(Queen) },
            { nameof(King)[0], nameof(King) },
        };

        public enum ColorType
        {
            White = 1,
            Black = 2,
            Empty = 0,
        }

        public class Coordinates<T> where T : struct
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
        public string coordinates;
        public readonly char pieceType;
        public readonly ColorType color;
        protected readonly ChessboardGenerator chessBoard;

        public Piece(ChessboardGenerator chessBoard, char pieceType)
        {
            this.chessBoard = chessBoard;
            this.pieceType = pieceType;
            this.color = char.IsUpper(pieceType) ? ColorType.White : ColorType.Black;
        }

        public static ColorType GetColorOfPieceFromFen(string fen, string coordinates)
        {
            char? piece = FenHelper.GetPieceOnSquare(fen, coordinates);
            if (!piece.HasValue)
                return ColorType.Empty;

            return char.IsUpper(piece.Value) ? ColorType.White : ColorType.Black;
        }

        public static ColorType GetColorFromChessboard(ChessboardGenerator chessBoard)
        {
            return chessBoard.boardColorOrientation;
        }

        public static ColorType GetOppositeColor(ColorType colorType)
        {
            return colorType == ColorType.White ? ColorType.Black : ColorType.White;
        }

        public static ColorType GetColorOfPiece(char piece)
        {
            return char.IsUpper(piece) ? ColorType.White : ColorType.Black;
        }

        public abstract HashSet<string> GetAvailableMoves();

        public bool IsPieceByColorOnSquare(string fen, string squareCoordinates, ColorType pieceColor, out char? pieceType)
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
        /// <returns></returns>
        protected TryAddMoveResult TryAddMove<T>(HashSet<string> availableMoves, T chessBoard, string squareCoordinates) where T : ChessboardGenerator
        {
            if (string.IsNullOrEmpty(squareCoordinates))
            {
                return new TryAddMoveResult()
                {
                    isPieceOnSquare = false,
                    success = false,
                };
            }

            chessBoard.pieces.TryGetValue(squareCoordinates, out Piece pieceOnSquare);
            bool isPieceOnSquare = pieceOnSquare != null;
            bool isEnemyPieceOnSquare = isPieceOnSquare && pieceOnSquare.color != color;

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
