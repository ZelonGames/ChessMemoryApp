using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class ThreatCalculationTeacher
    {
        private readonly UIChessBoard chessBoard;
        private readonly Dictionary<string, string> chosenThreats = new();

        public ThreatCalculationTeacher(UIChessBoard chessBoard, MoveNotationGenerator moveNotationGenerator)
        {
            this.chessBoard = chessBoard;
            moveNotationGenerator.MoveNotationCompleted += OnMoveNotationCompleted;
        }

        private void OnMoveNotationCompleted(string firstClick, string secondClick)
        {
            chosenThreats.Add(chessBoard.chessBoardData.GetPositionFen(), firstClick + secondClick);
        }

        public void CalculateNextMove()
        {

        }
    }
}
