using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    public class MoveNotationValidator
    {
        public delegate void ValidatedMoveNotationHandler(string fromCoordinates, string toCoordinates);
        public event ValidatedMoveNotationHandler AcceptedMoveNotation;
        public event ValidatedMoveNotationHandler RejectedMoveNotation;

        private readonly UIChessBoard chessboard;
        private readonly MoveNotationGenerator moveNotationGenerator;

        public MoveNotationValidator(UIChessBoard chessboard)
        {
            this.chessboard = chessboard;
            this.moveNotationGenerator = new MoveNotationGenerator(chessboard);
            moveNotationGenerator.MoveNotationCompleted += OnMoveNotationCompleted;
        }

        private void OnMoveNotationCompleted(string firstClick, string secondClick)
        {
            if (IsMoveNotationCoordinatesLegal(chessboard.chessBoardData, firstClick + secondClick))
                AcceptedMoveNotation?.Invoke(firstClick, secondClick);
            else
            {
                moveNotationGenerator.ResetClicks();
                RejectedMoveNotation?.Invoke(firstClick, secondClick);
            }
        }

        public bool IsMoveNotationCoordinatesLegal(ChessboardGenerator chessboard, string moveNotationCoordinates)
        {
            string fromCoordinates = moveNotationCoordinates[..2];
            string toCoordinates = moveNotationCoordinates[2..];

            Piece movingPiece = chessboard.GetPiece(fromCoordinates);
            return movingPiece != null && movingPiece.GetAvailableMoves().Contains(toCoordinates);
        }
    }
}
