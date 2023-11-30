using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Opening_Practice
{
    public class OpeningPracticeInputController
    {
        private ChessEngine chessEngine;

        public OpeningPracticeInputController(ChessEngine chessEngine, UIChessBoard chessBoardUI) 
        {
            this.chessEngine = chessEngine;
            var moveNotationGenerator = new MoveNotationGenerator(chessBoardUI);
            moveNotationGenerator.MoveNotationCompleted += OnMoveNotationCompleted;
        }

        private async void OnMoveNotationCompleted(string firstClick, string secondClick)
        {

            chessEngine.MakeMove(firstClick + secondClick);

            string engineMove = await chessEngine.GetNextEngineMoveFromCurrentFEN();
            chessEngine.MakeMove(engineMove);
        }
    }
}
