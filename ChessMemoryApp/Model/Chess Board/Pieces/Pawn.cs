using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessMemoryApp.Model.CourseMaker;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'p')
        {

        }
        public static HashSet<string> GetAvailableMoves(string pieceLetterCoordinates, string fen)
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(pieceLetterCoordinates);
            char? pawn = FenHelper.GetPieceOnSquare(fen, pieceLetterCoordinates);
            bool isWhite = char.IsUpper(pawn.Value);
            ColorType pieceColor = isWhite ? Piece.ColorType.White : Piece.ColorType.Black;

            // Forward
            string currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X, false);
            char? enemyPiece = FenHelper.GetPieceOnSquare(fen, currentCoordinates);

            // Normally a move would be added if an enemy piece is on the square but pawns can't capture forward
            bool isPieceOnSquare = FenHelper.GetPieceOnSquare(fen, currentCoordinates).HasValue;
            if (!isPieceOnSquare)
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);

            // Two moves forward
            char startingRow = isWhite ? '2' : '7';
            bool hasMoved = pieceLetterCoordinates[1] != startingRow;
            if (!hasMoved)
            {
                string betweenRow = isWhite ? "3" : "6";
                char file = pieceLetterCoordinates[0];
                bool isAnyPieceBetween = FenHelper.GetPieceOnSquare(fen, file + betweenRow).HasValue;
                if (!isAnyPieceBetween)
                {
                    currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X, true);
                    isPieceOnSquare = FenHelper.GetPieceOnSquare(fen, currentCoordinates).HasValue;
                    if (!isPieceOnSquare)
                        TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);
                }
            }

            // Capture Left
            currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X - 1, false);
            char? piece = FenHelper.GetPieceOnSquare(fen, currentCoordinates);
            if (piece.HasValue)
            {
                bool isPieceAnEnemy = isWhite ? char.IsLower(piece.Value) : char.IsUpper(piece.Value);
                if (isPieceAnEnemy)
                    TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);
            }

            // Capture Right
            currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X + 1, false);
            piece = FenHelper.GetPieceOnSquare(fen, currentCoordinates);
            if (piece.HasValue)
            {
                bool isPieceAnEnemy = isWhite ? char.IsLower(piece.Value) : char.IsUpper(piece.Value);
                if (isPieceAnEnemy)
                    TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates);
            }

            // En Passant
            string enPassantSquare = FenHelper.GetEnPassantSquareFromFen(fen);
            if (enPassantSquare != null)
                availableMoves.Add(enPassantSquare);

            return availableMoves;
        }

        private static string GetForwardMove(bool isWhite, Coordinates<int> pieceCoordinates, int coordinateX, bool isDoubleMove)
        {
            if (isWhite)
            {
                if (isDoubleMove)
                    return BoardHelper.GetLetterCoordinates(new Coordinates<int>(coordinateX, pieceCoordinates.Y + 2));
                else
                    return BoardHelper.GetLetterCoordinates(new Coordinates<int>(coordinateX, pieceCoordinates.Y + 1));
            }
            else
            {
                if (isDoubleMove)
                    return BoardHelper.GetLetterCoordinates(new Coordinates<int>(coordinateX, pieceCoordinates.Y - 2));
                else
                    return BoardHelper.GetLetterCoordinates(new Coordinates<int>(coordinateX, pieceCoordinates.Y - 1));
            }
        }
    }
}
