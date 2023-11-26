using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Memory_Techniques
{
    /// <summary>
    /// Generates buttons from the move notations input text
    /// </summary>
    public static class MoveNotationButtonGenerator
    {
        public static void GenerateButtons(string moveNotationsText, AbsoluteLayout layoutWordButtons)
        {
            layoutWordButtons.Clear();

            string[] moveNotations = moveNotationsText.Split(' ');

            int marginWidth = 5;
            int marginHeight = 5;

            int currentColumn = 0;
            int currentRow = 0;
            double layoutWidth = layoutWordButtons.Bounds.Width;
            double currentTotalWidth = 0;

            foreach (string moveNotation in moveNotations)
            {
                var button = new UIMemoryPalaceWordButton(moveNotation);
                
                if (currentTotalWidth >= layoutWidth - button.button.Width)
                {
                    currentTotalWidth = 0;
                    currentColumn = 0;
                    currentRow++;
                }

                int smartMarginWidth = currentColumn == 0 ? 0 : marginWidth * currentColumn;
                button.button.TranslationX = button.button.Width * currentColumn + smartMarginWidth;
                button.button.TranslationY = button.button.Height * currentRow + marginHeight * currentRow;
                currentTotalWidth += button.button.WidthRequest + marginWidth;
                currentColumn++;
                layoutWordButtons.Add(button.button);
            }
        }
    }
}
