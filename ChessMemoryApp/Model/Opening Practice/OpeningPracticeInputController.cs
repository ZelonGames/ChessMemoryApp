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
        private ChessBot chessBot;

        public OpeningPracticeInputController(ChessBot chessBot, UIChessBoard chessBoardUI)
        {
            this.chessBot = chessBot;
            var moveNotationValidator = new MoveNotationValidator(chessBoardUI);
            moveNotationValidator.AcceptedMoveNotation += OnAcceptedMoveNotation;
        }

        private async void OnAcceptedMoveNotation(string fromCoordinates, string toCoordinates)
        {
            if (chessBot.IsThinking)
                return;

            chessBot.MakeMove(fromCoordinates + toCoordinates);

            string engineMove = await chessBot.GetNextEngineMoveFromCurrentFEN();
            chessBot.MakeMove(engineMove);
        }
    }
}
