using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Memory_Techniques
{
    /// <summary>
    /// Loads words to the corresponding button
    /// </summary>
    public static class ButtonWordLoader
    {
        private static NumberToWordConverter numberToWordConverter;
        private static readonly Dictionary<string, List<string>> hashedWords = new();

        public static void Install(List<UIMemoryPalaceWordButton> memoryPalaceWordButtons, NumberToWordConverter numberToWordConverter)
        {
            ButtonWordLoader.numberToWordConverter = numberToWordConverter;

            foreach (UIMemoryPalaceWordButton button in memoryPalaceWordButtons)
            {
                button.PointerEntered += OnPointerEnteredButton;
            }
        }

        private static void OnPointerEnteredButton(UIMemoryPalaceWordButton interactedButton)
        {
            if (!hashedWords.ContainsKey(interactedButton.button.Text))
            {
                List<string> words = numberToWordConverter.GetWordsFromChessCoordinates(interactedButton.button.Text);
                hashedWords.Add(interactedButton.button.Text, words);
            }
        }
    }
}
