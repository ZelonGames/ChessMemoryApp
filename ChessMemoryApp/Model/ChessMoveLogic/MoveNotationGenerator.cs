using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using ChessMemoryApp.Model.UI_Components;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    public class MoveNotationGenerator
    {
        public delegate void MoveNotationCompletedEventHandler(string firstClick, string secondClick);
        public event MoveNotationCompletedEventHandler MoveNotationCompleted;

        public readonly UIChessBoard chessBoard;
        private string firstClick = null;
        private string secondClick = null;

        private bool IsFirstClick => firstClick == null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chessBoard">The chess board to generate move notations on</param>
        public MoveNotationGenerator(UIChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
            this.chessBoard.pieceUICollection.ClickedPiece += OnClickedPiece;
            this.chessBoard.squareCollection.ClickedSquare += OnClickedSquare;
        }

        private void OnClickedSquare(Square clickedSquare)
        {
            // The first click has to be on a piece
            if (!IsFirstClick)
            {
                SetSecondClick(BoardHelper.GetNumberCoordinates(clickedSquare.coordinates));
                Square.HighlightedSquare?.LowlightSquare();
            }
        }

        private void OnClickedPiece(PieceUI clickedPiece)
        {
            if (IsFirstClick)
            {
                SetFirstClick(BoardHelper.GetNumberCoordinates(clickedPiece.coordinates));
                chessBoard.squareCollection.GetSquare(clickedPiece.coordinates).HighlightSquare();
            }
            else
            {
                SetSecondClick(BoardHelper.GetNumberCoordinates(clickedPiece.coordinates));
                Square.HighlightedSquare?.LowlightSquare();
            }
        }


        public void ResetClicks()
        {
            firstClick = secondClick = null;
        }

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

        private void SetFirstClick(Piece.Coordinates<int> coordinate)
        {
            string letterCoordinate = BoardHelper.GetLetterCoordinates(coordinate);
            firstClick = letterCoordinate;
        }

        private void SetSecondClick(Piece.Coordinates<int> coordinate, bool selectedPiece = false)
        {
            if (UIHelper.GetCheckBox_PlayAsBlack_Value())
            {
                if (!selectedPiece)
                    coordinate = new Piece.Coordinates<int>(9 - coordinate.X, 9 - coordinate.Y);
            }

            string letterCoordinate = BoardHelper.GetLetterCoordinates(coordinate);
            secondClick = letterCoordinate;
            MoveNotationCompleted?.Invoke(firstClick, secondClick);
            ResetClicks();
        }
    }
}
