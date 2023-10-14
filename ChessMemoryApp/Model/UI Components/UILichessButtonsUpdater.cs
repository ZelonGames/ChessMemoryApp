using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public static class UILichessButtonsUpdater
    {
        private static CustomVariation customVariation;
        private static LichessMoveLoader lichessMoveLoader;

        public static void Install(CustomVariation customVariation, LichessMoveLoader lichessMoveLoader)
        {
            UILichessButtonsUpdater.customVariation = customVariation;
            UILichessButtonsUpdater.lichessMoveLoader = lichessMoveLoader;

            lichessMoveLoader.FinishedLoadingLichess += OnFinishedLoadingLichess;
        }

        private static void OnFinishedLoadingLichess()
        {
            if (customVariation.moves.Count == 0)
                return;

            int plyMoves = customVariation.moves.Count + FenHelper.GetAmountOfPlayedPlyMoves(customVariation.Course.PreviewFen);
            foreach (LichessButton lichessButton in lichessMoveLoader.lichessButtons)
            {
                bool isValidLichessButton = false;
                foreach (Chapter chapter in customVariation.Course.GetChapters().Values)
                {
                    foreach (Variation variation in chapter.GetVariations().Values)
                    {
                        bool isCorrectVariation = 
                            variation.moves.Count > plyMoves &&
                            variation.moves[plyMoves - 1].MoveNotation == 
                            customVariation.moves.Last().moveNotation;
                        if (!isCorrectVariation)
                            continue;

                        if (variation.moves[plyMoves].MoveNotation == lichessButton.move.MoveNotation)
                        {
                            isValidLichessButton = true;
                            break;
                        }
                    }

                    if (isValidLichessButton)
                        break;
                }

                if (!isValidLichessButton)
                {
                    lichessButton.listButton.isRedMarked = true;
                    lichessButton.listButton.UpdateColor();
                }
            }
        }
    }
}
