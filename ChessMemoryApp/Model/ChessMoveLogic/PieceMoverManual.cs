using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.UI_Components;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    /// <summary>
    /// Moves the pieces when a user guessed the correct move
    /// </summary>
    public class PieceMoverManual
    {
        public delegate void MovedPieceEventHandler(string fen);
        public event MovedPieceEventHandler MovedPiece;

        public delegate void MadeMoveFenEventHandler(MoveHistory.MoveSource moveSource, Piece.ColorType color, string moveNotation, string previousFen, string currentFen);
        public event MadeMoveFenEventHandler MadeMoveFen;

        public readonly MoveNotationGenerator moveNotationGenerator;
        private readonly UIChessBoard chessBoard;

        public PieceMoverManual(UIChessBoard chessBoard, MoveNotationGenerator moveNotationGenerator, CustomVariationMoveNavigator customVariationMoveNavigator)
        {
            this.chessBoard = chessBoard;
            this.moveNotationGenerator = moveNotationGenerator;
            customVariationMoveNavigator.GuessedCorrectMove += OnGuessedCorrectMove;
            customVariationMoveNavigator.RevealedMove += OnRevealedMove;
        }

        private void OnRevealedMove(string fen)
        {
            chessBoard.chessBoardData.AddPiecesFromFen(fen);
        }

        private void OnGuessedCorrectMove(MoveHistory.Move moveToMake)
        {
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveToMake.moveNotationCoordinates);
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveToMake.moveNotationCoordinates);
            string fen = chessBoard.chessBoardData.GetPositionFen();
            chessBoard.chessBoardData.MakeMove(fromCoordinates + toCoordinates);
            MovedPiece?.Invoke(fen);
            moveNotationGenerator.ResetClicks();
        }
    }
}
