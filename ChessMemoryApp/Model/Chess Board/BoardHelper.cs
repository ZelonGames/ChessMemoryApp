using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;

namespace ChessMemoryApp.Model.Chess_Board
{
    public static class BoardHelper
    {
        public static Piece.ColorType piecesToMove = Piece.ColorType.White;

        /// <summary>
        /// replaces the x coordinate with the corresponding letter
        /// </summary>
        /// <returns></returns>
        public static string GetLetterCoordinates(Piece.Coordinates<int> coordinate)
        {
            if (coordinate.X < 1 || coordinate.Y < 1 || coordinate.X > 8 || coordinate.Y > 8)
                return "";

            string letterCoordinates = "abcdefgh";

            return letterCoordinates[coordinate.X - 1].ToString() + coordinate.Y;
        }

        public static Piece.Coordinates<int> GetNumberCoordinates(string letterCoordinates)
        {
            string allLetterCoordinates = "abcdefgh";

            int letterCoordinate = letterCoordinates.Length == 3 ? allLetterCoordinates.IndexOf(letterCoordinates[1]) + 1 : allLetterCoordinates.IndexOf(letterCoordinates[0]) + 1;
            char yCoordinate = letterCoordinates.Length == 3 ? letterCoordinates[2] : letterCoordinates[1];

            return new Piece.Coordinates<int>(letterCoordinate, (int)char.GetNumericValue(yCoordinate));
        }

        public static Piece.Coordinates<char> GetCoordinates(string moveNotation)
        {
            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            if (moveNotation.Contains('x'))
                moveNotation = moveNotation.Split('x')[1];

            char x = moveNotation[moveNotation.Length - 2];
            char y = moveNotation[moveNotation.Length - 1];

            return new Piece.Coordinates<char>(x, y);
        }

        public static Piece.Coordinates<char> GetFromCoordinates(string numberCoordinates)
        {
            numberCoordinates = GetFromCoordinatesString(numberCoordinates);
            char x = numberCoordinates[0];
            char y = numberCoordinates[1];

            return new Piece.Coordinates<char>(x, y);
        }

        public static string GetFromCoordinatesString(string numberCoordinates)
        {
            return numberCoordinates.Substring(0, 2);
        }

        public static Piece.Coordinates<char> GetToCoordinates(string numberCoordinates)
        {
            numberCoordinates = GetToCoordinatesString(numberCoordinates);
            char x = numberCoordinates[0];
            char y = numberCoordinates[1];

            return new Piece.Coordinates<char>(x, y);
        }

        public static string GetToCoordinatesString(string numberCoordinates)
        {
            return numberCoordinates.Substring(numberCoordinates.Length - 2);
        }
    }
}

