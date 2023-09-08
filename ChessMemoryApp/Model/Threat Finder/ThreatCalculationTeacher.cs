using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class ThreatCalculationTeacher
    {
        private readonly ChessboardGenerator chessBoard;
        private readonly Dictionary<string, string> chosenThreats = new();

        public ThreatCalculationTeacher(ChessboardGenerator chessBoard)
        {
            this.chessBoard = chessBoard;
            chessBoard.moveNotationHelper.MoveNotationCompleted += OnMoveNotationCompleted;
        }

        private void OnMoveNotationCompleted(string firstClick, string secondClick)
        {
            chosenThreats.Add(chessBoard.GetFen(), firstClick + secondClick);
        }

        public void CalculateNextMove()
        {

        }
    }
}
