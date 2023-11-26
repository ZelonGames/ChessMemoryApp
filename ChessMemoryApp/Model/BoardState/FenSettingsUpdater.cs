using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.ChessMoveLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;

namespace ChessMemoryApp.Model.BoardState
{
    /// <summary>
    /// Updates fen settings after a move has been made
    /// </summary>
    public class FenSettingsUpdater
    {
        protected FenSettings fenSettings;

        public FenSettingsUpdater(FenSettings fenSettings)
        {
            this.fenSettings = fenSettings;
        }

        protected void UpdatePawnPlyCount(string moveNotation)
        {
            bool isPawnMove = true;
            bool shouldReset;

            if (moveNotation.Contains('x'))
                shouldReset = true;
            else
            {
                foreach (var character in moveNotation.Split('x')[0])
                {
                    if (char.IsUpper(character))
                    {
                        isPawnMove = false;
                        break;
                    }
                }

                shouldReset = isPawnMove;
            }

            if (shouldReset)
                fenSettings.ResetPlyCount();
            else
                fenSettings.IncreasePlyCount();
        }

        protected void UpdateEnPassant(string moveNotation, string fen)
        {
            bool isDoubleMove = false;

            if (!isDoubleMove || fenSettings.CanEnPassant || moveNotation.Contains("="))
            {
                fenSettings.DisableEnPassant();
                return;
            }

            string[] moveNotationComponents = moveNotation.Replace("+", "").Replace("#", "").Split('x');
            string toCoordinates;
            if (moveNotationComponents.Length == 2)
                toCoordinates = moveNotationComponents[1];
            else
                toCoordinates = moveNotationComponents[0];

            bool isPawnMove = moveNotation.Length != 2;
            if (isPawnMove)
                return;

            Piece.Coordinates<int> coordinates = BoardHelper.GetNumberCoordinates(moveNotation);

            bool IsSameColor(char c1, char c2)
            {
                return char.IsUpper(c1) && char.IsUpper(c2) ||
                    char.IsLower(c1) && char.IsLower(c2);
            }

            bool IsNeighboorPieceEnemyPawn(char movedPiece, char? neighboorPiece)
            {
                bool isNeighboorPiecePawn = char.ToLower(neighboorPiece.Value) == 'p';
                return isNeighboorPiecePawn && !IsSameColor(neighboorPiece.Value, movedPiece);
            }

            string GetEnPassantSquare(bool isMovedPieceWhite, string movedPieceCoordinates)
            {
                Piece.Coordinates<int> coordinates = BoardHelper.GetNumberCoordinates(movedPieceCoordinates);

                if (isMovedPieceWhite)
                    return BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(coordinates.X, coordinates.Y - 1));
                else
                    return BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(coordinates.X, coordinates.Y + 1));
            }

            // Left side
            char movedPiece = FenHelper.GetPieceOnSquare(fen, toCoordinates).Value;
            string neighboorPieceCoordinates = BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(coordinates.X - 1, coordinates.Y));
            char? neighboorPiece = FenHelper.GetPieceOnSquare(fen, neighboorPieceCoordinates);
            bool isMovedPieceWhite = char.IsUpper(movedPiece);
            if (neighboorPiece.HasValue && IsNeighboorPieceEnemyPawn(movedPiece, neighboorPiece.Value))
                fenSettings.SetEnPassantSquare(GetEnPassantSquare(isMovedPieceWhite, toCoordinates));

            // Right Side
            movedPiece = FenHelper.GetPieceOnSquare(fen, toCoordinates).Value;
            neighboorPieceCoordinates = BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(coordinates.X + 1, coordinates.Y));
            neighboorPiece = FenHelper.GetPieceOnSquare(fen, neighboorPieceCoordinates);
            isMovedPieceWhite = char.IsUpper(movedPiece);
            if (neighboorPiece.HasValue && IsNeighboorPieceEnemyPawn(movedPiece, neighboorPiece.Value))
                fenSettings.SetEnPassantSquare(GetEnPassantSquare(isMovedPieceWhite, toCoordinates));
        }
    }
}
