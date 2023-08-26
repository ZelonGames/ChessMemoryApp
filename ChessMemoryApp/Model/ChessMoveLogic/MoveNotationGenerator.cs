using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    public class MoveNotationGenerator
    {
        public delegate void MoveNotationCompletedEventHandler(string firstClick, string secondClick);
        public event MoveNotationCompletedEventHandler MoveNotationCompleted;

        public readonly ChessboardGenerator chessBoard;
        private string firstClick = null;
        private string secondClick = null;

        public MoveNotationGenerator(ChessboardGenerator chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        public bool IsFirstClick => firstClick == null;

        public string ConvertCastleMoveNotation()
        {
            return "";
        }

        public void ResetClicks()
        {
            firstClick = secondClick = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribers">CustomVariationMoveNavigator</param>
        public void SubscribeToEvents(CustomVariationMoveNavigator customVariationMoveNavigator)
        {
            customVariationMoveNavigator.GuessedWrongMove += OnResetMove;
            customVariationMoveNavigator.GuessedCorrectMove += OnResetMove;
            customVariationMoveNavigator.GuessedLastMove += OnResetMove;
        }

        private void OnResetMove(MoveHistory.Move moveToMake)
        {
            firstClick = secondClick = null;
        }

        public void SetFirstClick(Piece.Coordinates<int> coordinate)
        {
            string letterCoordinate = BoardHelper.GetLetterCoordinates(coordinate);
            firstClick = letterCoordinate;
        }

        public void SetSecondClick(Piece.Coordinates<int> coordinate, bool selectedPiece = false)
        {
            if (UIHelper.GetCheckBox_PlayAsBlack_Value())
            {
                if (!selectedPiece)
                    coordinate = new Piece.Coordinates<int>(9 - coordinate.X, 9 - coordinate.Y);
            }

            string letterCoordinate = BoardHelper.GetLetterCoordinates(coordinate);
            secondClick = letterCoordinate;
            MoveNotationCompleted?.Invoke(firstClick, secondClick);
        }

    }

    public interface IMoveNotationHelper
    {
        void ResetEvents();
        void OnPictureBoxClicked(object sender, EventArgs e);
    }
}
