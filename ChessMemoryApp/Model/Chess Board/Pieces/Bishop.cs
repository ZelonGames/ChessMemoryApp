﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'b')
        {

        }

        public static HashSet<string> GetAvailableMoves(string pieceLetterCoordinates, string fen)
        {
            var availableMoves = new HashSet<string>();
            Coordinates<int> pieceCoordinates = BoardHelper.GetNumberCoordinates(pieceLetterCoordinates);

            // Going up and right
            for (int x = pieceCoordinates.X + 1, y = pieceCoordinates.Y + 1; x <= 8 && y <= 8; x++, y++)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            // Going up and left
            for (int x = pieceCoordinates.X - 1, y = pieceCoordinates.Y + 1; x >= 1 && y <= 8; x--, y++)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            // Going down and right
            for (int x = pieceCoordinates.X + 1, y = pieceCoordinates.Y - 1; x <= 8 && y >= 1; x++, y--)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            // Going down and left
            for (int x = pieceCoordinates.X - 1, y = pieceCoordinates.Y - 1; x >= 1 && y >= 1; x--, y--)
            {
                string currentCoordinates = BoardHelper.GetLetterCoordinates(new Coordinates<int>(x, y));
                TryAddMove(availableMoves, fen, pieceLetterCoordinates, currentCoordinates, out var isPieceOnSquare);
                if (isPieceOnSquare)
                    break;
            }

            return availableMoves;
        }
    }
}
