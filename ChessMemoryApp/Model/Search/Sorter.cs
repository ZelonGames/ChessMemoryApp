using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Search
{
    public static class Sorter
    {
        public static void SortByAmountOfPlayedMoves(this List<(Variation variation, Move move)> filteredVariations, (Variation variation, Move lastSearchMove) foundVariation)
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
    }
}
