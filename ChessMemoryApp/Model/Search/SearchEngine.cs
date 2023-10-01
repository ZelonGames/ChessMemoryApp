using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Search
{
    public class SearchEngine
    {
        public readonly string[] moveNotations;
        public readonly string[] excludingMoveNotations;

        private Course course;
        private int plyMoveLimit;

        public SearchEngine(Course course, string searchPattern)
        {
            this.course = course;
            moveNotations = SearchEngineHelper.GetMoveNotations(searchPattern);
            excludingMoveNotations = SearchEngineHelper.GetExcludingMoveNotations(searchPattern);

            string[] inputParts = searchPattern.Split(SearchEngineHelper.MOVE_LIMIT_SEPARATOR);
            plyMoveLimit = inputParts.Length >= 2 ? Convert.ToInt32(inputParts[1]) : 0;
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

                    if (!SearchEngineHelper.IsCorrectMove(move, currentMoveNotation))
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
                if (Excluder.ShouldExcludeVariation(foundVariation, excludingMoveNotations))
                    continue;

                filteredVariations.SortByAmountOfPlayedMoves(foundVariation);
            }

            return filteredVariations;
        }

        public string GetLastMoveNotation()
        {
            return moveNotations.Last().IgnoreSearchPrefixes();
        }
    }
}
