using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Search
{
    public class Excluder
    {
        public static bool ShouldExcludeVariation((Variation variation, Move move) foundVariation, string[] excludingMoveNotations)
        {
            int lastSearchMoveIndex = foundVariation.variation.moves.IndexOf(foundVariation.move);

            foreach (var excludingMoveNotation in excludingMoveNotations)
            {
                bool isCapturingPieceUnspecified = SearchEngineHelper.IsCapturingPieceUnSpecified(excludingMoveNotation);
                foreach (var move in foundVariation.variation.moves)
                {
                    string moveNotation = move.MoveNotation;

                    if (isCapturingPieceUnspecified && SearchEngineHelper.IsCapturingMove(move.MoveNotation))
                        moveNotation = SearchEngineHelper.GetCapturedPieceNotation(move.MoveNotation);

                    if (moveNotation != SearchEngineHelper.IgnoreSearchPrefixes(excludingMoveNotation))
                        continue;

                    if (SearchEngineHelper.HasSpecifiedColor(excludingMoveNotation))
                    {
                        if (SearchEngineHelper.IsMoveCorrectColor(excludingMoveNotation, move))
                        {
                            int lastExcludedMoveIndex = foundVariation.variation.moves.IndexOf(move);
                            return lastExcludedMoveIndex < lastSearchMoveIndex;
                        }
                    }
                    else
                    {
                        int lastExcludedMoveIndex = foundVariation.variation.moves.IndexOf(move);
                        return lastExcludedMoveIndex < lastSearchMoveIndex;
                    }
                }
            }

            return false;
        }
    }
}
