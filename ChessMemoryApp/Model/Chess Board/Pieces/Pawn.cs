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
        public Pawn(ChessboardGenerator chessBoard, char pieceType) : base(chessBoard, pieceType)
        {

        }

        public override HashSet<string> GetAvailableMoves()
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(coordinates);

            // Forward
            string currentCoordinates = GetForwardMove(pieceCoordinates.X, false);

            // Normally a move would be added if an enemy piece is on the square but pawns can't capture forward
            bool isPieceOnSquare = chessBoard.GetPiece(currentCoordinates) != null;
            if (!isPieceOnSquare)
                TryAddMove(availableMoves, chessBoard, currentCoordinates);

            // Two moves forward
            char startingRow = color == ColorType.White ? '2' : '7';
            bool hasMoved = coordinates[1] != startingRow;
            if (!hasMoved)
            {
                string betweenRow = color == ColorType.White ? "3" : "6";
                char file = coordinates[0];
                bool isAnyPieceBetween = chessBoard.GetPiece(file + betweenRow) != null;
                if (!isAnyPieceBetween)
                {
                    currentCoordinates = GetForwardMove(pieceCoordinates.X, true);
                    isPieceOnSquare = chessBoard.GetPiece(currentCoordinates) != null;
                    if (!isPieceOnSquare)
                        TryAddMove(availableMoves, chessBoard, currentCoordinates);
                }
            }

            // Capture Left
            currentCoordinates = GetForwardMove(pieceCoordinates.X - 1, false);
            Piece piece = chessBoard.GetPiece(currentCoordinates);
            if (piece != null)
            {
                bool isPieceAnEnemy = piece.color != color;
                if (isPieceAnEnemy)
                    TryAddMove(availableMoves, chessBoard, currentCoordinates);
            }

            // Capture Right
            currentCoordinates = GetForwardMove(pieceCoordinates.X + 1, false);
            piece = chessBoard.GetPiece(currentCoordinates);
            if (piece != null)
            {
                bool isPieceAnEnemy = piece.color != color;
                if (isPieceAnEnemy)
                    TryAddMove(availableMoves, chessBoard, currentCoordinates);
            }

            // En Passant
            string enPassantSquare = FenHelper.GetEnPassantSquareFromFen(chessBoard.GetPositionFen());
            if (enPassantSquare != null)
                availableMoves.Add(enPassantSquare);

            return availableMoves;
        }

        private string GetForwardMove(int xCoordinate, bool isDoubleMove)
        {
            int yCoordinate = BoardHelper.GetNumberCoordinates(coordinates).Y;
            int steps = isDoubleMove ? 2 : 1;
            steps *= color == ColorType.White ? 1 : -1;
            return BoardHelper.GetLetterCoordinates(new Coordinates<int>(xCoordinate, yCoordinate + steps));
        }
    }
}
