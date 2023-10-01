using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Search
{
    public static class SearchEngineHelper
    {
        public const char MOVE_LIMIT_SEPARATOR = '|';
        private const char PREFIX_EXCLUDE = '!';
        private const char PREFIX_WHITE = 'w';
        private const char PREFIX_BLACK = '0';

        public static string[] GetMoveNotations(string moveNotation)
        {
            return moveNotation.Split(MOVE_LIMIT_SEPARATOR)[0].Split(' ').Where(x => x[0] != PREFIX_EXCLUDE).ToArray();
        }

        public static string[] GetExcludingMoveNotations(string moveNotationText)
        {
            return moveNotationText.Split(MOVE_LIMIT_SEPARATOR)[0].Split(' ').Where(x => x[0] == PREFIX_EXCLUDE).ToArray();
        }

        public static string IgnoreSearchPrefixes(this string moveNotation)
        {
            string pattern = $"{PREFIX_BLACK}|{PREFIX_WHITE}|{PREFIX_EXCLUDE}";
            return Regex.Replace(moveNotation, pattern, "");
        }

        public static string IgnoreExcludePrefix(string moveNotation)
        {
            return moveNotation.Replace(Convert.ToString(PREFIX_EXCLUDE), "");
        }

        public static string GetCapturedPieceNotation(string moveNotation)
        {
            string[] moveNotationParts = moveNotation.Split('x');
            if (moveNotationParts.Length != 2)
                return moveNotation;

            return 'x' + moveNotationParts[1];
        }

        public static bool IsCorrectMove(Move move, string currentMoveNotation)
        {
            if (IsCapturingPieceUnSpecified(currentMoveNotation) &&
                GetCapturedPieceNotation(move.MoveNotation) == currentMoveNotation.IgnoreSearchPrefixes())
                return true;
            else if (move.MoveNotation != currentMoveNotation.IgnoreSearchPrefixes())
                return false;

            if (HasSpecifiedColor(currentMoveNotation) &&
                !IsMoveCorrectColor(currentMoveNotation, move))
                return false;

            return true;
        }

        public static bool HasSpecifiedColor(string moveNotation)
        {
            return IgnoreExcludePrefix(moveNotation)[0] is PREFIX_BLACK or PREFIX_WHITE;
        }

        public static bool IsMoveCorrectColor(string moveNotation, Move move)
        {
            return (IgnoreExcludePrefix(moveNotation)[0], move.Color) is
                (PREFIX_BLACK, Piece.ColorType.Black) or
                (PREFIX_WHITE, Piece.ColorType.White);
        }

        public static bool IsCapturingPieceUnSpecified(string moveNotation)
        {
            return IgnoreSearchPrefixes(moveNotation)[0] == 'x';
        }

        public static bool IsCapturingMove(string moveNotation)
        {
            return moveNotation.Contains('x');
        }
    }
}
