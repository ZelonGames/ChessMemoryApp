using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    public class CustomVariationMoveNavigator
    {
        public delegate void RevealedMoveEventHandler(string fen);
        public delegate void GuessedMoveEventHandler(MoveHistory.Move moveToMake);
        public event GuessedMoveEventHandler GuessedCorrectMove;
        public event GuessedMoveEventHandler GuessedLastMove;
        public event GuessedMoveEventHandler GuessedWrongMove;
        public event RevealedMoveEventHandler RevealedMove;

        private readonly CustomVariation customVariation;
        private int currentMove = 0;

        public CustomVariationMoveNavigator(CustomVariation customVariation)
        {
            this.customVariation = customVariation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribers">MoveNotationGenerator, Button</param>
        public void SubscribeToEvents(MoveNotationGenerator moveNotationGenerator, Button buttonStart, Button buttonPrevious, Button buttonNext, Button buttonEnd)
        {
            moveNotationGenerator.MoveNotationCompleted += MoveNotationGenerator_MoveNotationCompleted;
            buttonNext.Clicked += RevealNextMove;
            buttonEnd.Clicked += RevealLastMove;
            buttonPrevious.Clicked += RevealPreviousMove;
            buttonStart.Clicked += RevealFirstMove;
        }

        public void RevealNextMove(object sender, EventArgs args)
        {
            MoveHistory.Move nextMove = GetNextMove();
            if (nextMove == null)
                return;

            if (currentMove < customVariation.moves.Count - 1)
                currentMove++;
            RevealedMove?.Invoke(nextMove.fen);
        }

        public void RevealLastMove(object sender, EventArgs args)
        {
            MoveHistory.Move nextMove = GetLastMove();
            if (nextMove == null)
                return;

            currentMove = customVariation.moves.Count - 1;
            RevealedMove?.Invoke(nextMove.fen);
        }

        public void RevealPreviousMove(object sender, EventArgs args)
        {
            MoveHistory.Move nextMove = GetPreviousMove();
            if (nextMove == null)
                return;

            if (currentMove > 0)
                currentMove--;
            RevealedMove?.Invoke(nextMove.fen);
        }

        public void RevealFirstMove(object sender, EventArgs args)
        {
            MoveHistory.Move nextMove = GetFirstMove();
            if (nextMove == null)
                return;

            currentMove = 0;
            RevealedMove?.Invoke(nextMove.fen);
        }

        private void MoveNotationGenerator_MoveNotationCompleted(string firstClick, string secondClick)
        {
            MoveHistory.Move nextMove = GetNextMove();

            if (nextMove.moveNotationCoordinates == firstClick + secondClick)
            {
                if (currentMove < customVariation.moves.Count - 1)
                    currentMove++;
                GuessedCorrectMove?.Invoke(nextMove);
            }
            else
                GuessedWrongMove?.Invoke(nextMove);
        }

        private MoveHistory.Move GetNextMove()
        {
            if (currentMove + 1 < customVariation.moves.Count - 1)
                return customVariation.moves[currentMove + 1];

            return customVariation.moves.Last();
        }

        private MoveHistory.Move GetLastMove()
        {
            return customVariation.moves.Last();
        }

        private MoveHistory.Move GetPreviousMove()
        {
            if (currentMove - 1 >= 0)
                return customVariation.moves[currentMove - 1];

            return customVariation.moves.First();
        }

        private MoveHistory.Move GetFirstMove()
        {
            return customVariation.moves.First();
        }
    }
}
