using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace ChessMemoryApp.Model.CourseMaker
{
    public static class CourseHelper
    {
        public static bool BeginsWithDigitsAndDot(string input)
        {
            return Regex.IsMatch(input, @"^\d+\.[^\.].*$");
        }

        public static bool EndsWithNumbers(string content, string prefix)
        {
            string input = content;
            string pattern = $"^{Regex.Escape(prefix)} \\(\\d+\\)$";
            return Regex.IsMatch(input, pattern);
        }

        public static bool ContainsSquareBrackets(string input)
        {
            return Regex.IsMatch(input, @"\[[^\]]+\]");
        }

        public static char GetNextLetter(char letter)
        {
            letter = char.ToUpper(letter);

            if (letter == 'Z')
                return 'A';

            return (char)(letter + 1);
        }
    }
}
