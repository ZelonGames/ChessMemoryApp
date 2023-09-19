using ChessMemoryApp.Model.Chess_Board.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.CourseMaker
{
    public class SearchEngine
    {
        public const char MOVE_LIMIT_SEPARATOR = '|';
        private const char PREFIX_EXCLUDE = '!';
        private const char PREFIX_WHITE = 'w';
        private const char PREFIX_BLACK = '0';

        public readonly string[] moveNotations;
        public readonly string[] excludingMoveNotations;

        private Course course;
        private int plyMoveLimit;

        public SearchEngine(Course course, string searchPattern)
        {
            this.course = course;
            moveNotations = GetMoveNotations(searchPattern);
            excludingMoveNotations = GetExcludingMoveNotations(searchPattern);

            string[] inputParts = searchPattern.Split(MOVE_LIMIT_SEPARATOR);
            plyMoveLimit = inputParts.Length >= 2 ? Convert.ToInt32(inputParts[1]) : 0;
        }

        public static string[] GetMoveNotations(string moveNotation)
        {
            return moveNotation.Split(MOVE_LIMIT_SEPARATOR)[0].Split(' ').Where(x => x[0] != PREFIX_EXCLUDE).ToArray();
        }

        public static string[] GetExcludingMoveNotations(string moveNotationText)
        {
            return moveNotationText.Split(MOVE_LIMIT_SEPARATOR)[0].Split(' ').Where(x => x[0] == PREFIX_EXCLUDE).ToArray();
        }

        public static string IgnoreSearchPrefixes(string moveNotation)
        {
            string pattern = $"{PREFIX_BLACK}|{PREFIX_WHITE}|{PREFIX_EXCLUDE}";
            return Regex.Replace(moveNotation, pattern, "");
        }

        public Dictionary<string, (Variation, Move)> GetVariationsFromSearchPattern()
        {
            int currentComparingMoveNotationIndex = 0;

            var foundVariations = new Dictionary<string, (Variation, Move)>();
            var variations = course.GetChapters().SelectMany(x => x.Value.GetVariations());

            bool unlimitedPlyMove = plyMoveLimit == 0;

            #region Search Variations
            foreach (var variation in variations)
            {
                if (unlimitedPlyMove)
                    plyMoveLimit = variation.Value.moves.Count;

                int amountOfMoves = Math.Min(variation.Value.moves.Count, plyMoveLimit);
                for (int i = 0; i < amountOfMoves; i++)
                {
                    Move move = variation.Value.moves[i];
                    string currentMoveNotation = moveNotations[currentComparingMoveNotationIndex];

                    if (currentComparingMoveNotationIndex >= moveNotations.Length)
                        break;

                    if (IsCapturingPieceUnSpecified(currentMoveNotation) &&
                        CapturedPiece(move.MoveNotation) == IgnoreSearchPrefixes(currentMoveNotation))
                    {

                    }
                    else if (move.MoveNotation != IgnoreSearchPrefixes(currentMoveNotation))
                        continue;

                    if (HasSpecifiedColor(currentMoveNotation) &&
                        !IsMoveCorrectColor(currentMoveNotation, move))
                        continue;

                    currentComparingMoveNotationIndex++;
                    if (currentComparingMoveNotationIndex >= moveNotations.Length)
                    {
                        // A duplicate might be here so look at the first move notation next time
                        currentComparingMoveNotationIndex = 0;
                        if (foundVariations.TryAdd(move.Fen, (variation.Value, move)))
                            break;
                    }
                }

                // Look at the first move notation next time, if the variation wasn't correct
                currentComparingMoveNotationIndex = 0;
            }
            #endregion

            return foundVariations;
        }

        public List<(Variation variation, Move move)> ExcludeVariationsByMoveNotation(Dictionary<string, (Variation variation, Move lastSearchMove)> foundVariations)
        {
            var filteredVariations = new List<(Variation variation, Move move)>();

            foreach (var foundVariation in foundVariations.Values)
            {
                int lastSearchMoveIndex = foundVariation.variation.moves.IndexOf(foundVariation.lastSearchMove);

                #region Exclude Variations
                if (ShouldExcludeVariation(foundVariation.variation, excludingMoveNotations, out var excludedMove))
                {
                    int lastExcludedMoveIndex = foundVariation.variation.moves.IndexOf(excludedMove);

                    if (lastExcludedMoveIndex < lastSearchMoveIndex)
                        continue;
                }
                #endregion

                SortByAmountOfPlayedMoves(filteredVariations, foundVariation);
            }

            return filteredVariations;
        }

        private void FilterVariations()
        {

        }

        private void SortByAmountOfPlayedMoves(List<(Variation variation, Move move)> filteredVariations, (Variation variation, Move lastSearchMove) foundVariation)
        {
            int lastSearchMoveIndex = foundVariation.variation.moves.IndexOf(foundVariation.lastSearchMove);
            int insertIndex = filteredVariations.Count;

            for (int i = filteredVariations.Count - 1; i >= 0; i--)
            {
                int lastFilteredMoveIndex = filteredVariations[i].variation.moves.IndexOf(filteredVariations[i].move);

                if (lastSearchMoveIndex < lastFilteredMoveIndex)
                    insertIndex = i;
                else
                    break;
            }

            filteredVariations.Insert(insertIndex, (foundVariation.variation, foundVariation.lastSearchMove));
        }

        public string GetLastMoveNotation()
        {
            return IgnoreSearchPrefixes(moveNotations.Last());
        }

        private string IgnoreExcludePrefix(string moveNotation)
        {
            return moveNotation.Replace(Convert.ToString(PREFIX_EXCLUDE), "");
        }

        private string CapturedPiece(string moveNotation)
        {
            string[] moveNotationParts = moveNotation.Split('x');
            if (moveNotationParts.Length != 2)
                return moveNotation;

            return 'x' + moveNotationParts[1];
        }

        private bool ShouldExcludeVariation(Variation variation, string[] excludingMoveNotations, out Move excludedMove)
        {
            foreach (var excludingMoveNotation in excludingMoveNotations)
            {
                bool isCapturingPieceUnspecified = IsCapturingPieceUnSpecified(excludingMoveNotation);
                foreach (var move in variation.moves)
                {
                    string moveNotation = move.MoveNotation;

                    if (isCapturingPieceUnspecified && IsCapturingMove(move.MoveNotation))
                        moveNotation = CapturedPiece(move.MoveNotation);

                    if (moveNotation != IgnoreSearchPrefixes(excludingMoveNotation))
                        continue;

                    if (HasSpecifiedColor(excludingMoveNotation))
                    {
                        if (IsMoveCorrectColor(excludingMoveNotation, move))
                        {
                            excludedMove = move;
                            return true;
                        }
                    }
                    else
                    {
                        excludedMove = move;
                        return true;
                    }
                }
            }

            excludedMove = null;
            return false;
        }

        private bool HasSpecifiedColor(string moveNotation)
        {
            return IgnoreExcludePrefix(moveNotation)[0] is PREFIX_BLACK or PREFIX_WHITE;
        }

        private bool IsMoveCorrectColor(string moveNotation, Move move)
        {
            return (IgnoreExcludePrefix(moveNotation)[0], move.Color) is
                (PREFIX_BLACK, Piece.ColorType.Black) or
                (PREFIX_WHITE, Piece.ColorType.White);
        }

        private bool IsCapturingPieceUnSpecified(string moveNotation)
        {
            return IgnoreSearchPrefixes(moveNotation)[0] == 'x';
        }

        private bool IsCapturingMove(string moveNotation)
        {
            return moveNotation.Contains('x');
        }
    }
}
