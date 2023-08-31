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

            // Forward
            string currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X, false);
            TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);

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
                    TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);
                }
            }

            // Capture Left
            currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X - 1, false);
            char? piece = FenHelper.GetPieceOnSquare(fen, currentCoordinates);
            if (piece.HasValue)
            {
                bool isPieceAnEnemy = char.IsLower(piece.Value);
                if (isPieceAnEnemy)
                    TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);
            }

            // Capture Right
            currentCoordinates = GetForwardMove(isWhite, pieceCoordinates, pieceCoordinates.X + 1, false);
            piece = FenHelper.GetPieceOnSquare(fen, currentCoordinates);
            if (piece.HasValue)
            {
                bool isPieceAnEnemy = char.IsLower(piece.Value);
                if (isPieceAnEnemy)
                    TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out _);
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
