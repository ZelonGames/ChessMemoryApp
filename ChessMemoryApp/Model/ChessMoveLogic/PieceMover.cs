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
    /// Moves the pieces
    /// </summary>
    public class PieceMover
    {
        public delegate void MovedPieceEventHandler(string fen);
        public event MovedPieceEventHandler MovedPiece;

        public event Action<Move> MadeChessableMove;
        public event Action<Move> MadePreviousChessableMove;

        public delegate void MadeLichessMoveEventHandler(string fen, ExplorerMove move);
        public event MadeLichessMoveEventHandler MadeLichessMove;
        public event MadeLichessMoveEventHandler MadePreviousLichessMove;

        public delegate void MadeMoveFenEventHandler(MoveHistory.MoveSource moveSource, Piece.ColorType color, string moveNotation, string previousFen, string currentFen);
        public event MadeMoveFenEventHandler MadeMoveFen;

        public readonly MoveNotationGenerator moveNotationHelper;
        public readonly bool memorizingMode;

        private ChessboardGenerator ChessBoard => moveNotationHelper.chessBoard;

        public PieceMover(MoveNotationGenerator moveNotationHelper, bool memorizingMode)
        {
            this.moveNotationHelper = moveNotationHelper;
            this.memorizingMode = memorizingMode;
        }
        public PieceMover(MoveNotationGenerator moveNotationHelper, CustomVariationMoveNavigator customVariationMoveNavigator, bool memorizingMode)
            : this(moveNotationHelper, memorizingMode)
        {
            customVariationMoveNavigator.GuessedCorrectMove += CustomVariationMoveNavigator_GuessedCorrectMove;
            customVariationMoveNavigator.RevealedMove += CustomVariationMoveNavigator_RevealedMove;
        }

        private void CustomVariationMoveNavigator_RevealedMove(string fen)
        {
            ChessBoard.AddPiecesFromFen(fen);
        }

        public void SubscribeToEvents(CourseMoveNavigator courseMoveNavigator)
        {
            LichessButton.Clicked += OnNextLichessMove;
            courseMoveNavigator.RequestedNextChessableMove += OnNextChessableMove;
        }

        private void CustomVariationMoveNavigator_GuessedCorrectMove(MoveHistory.Move moveToMake)
        {
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveToMake.moveNotationCoordinates);
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveToMake.moveNotationCoordinates);
            string fen = ChessBoard.GetFen();
            ChessBoard.MakeMove(fromCoordinates + toCoordinates);
            MovedPiece?.Invoke(fen);
            moveNotationHelper.ResetClicks();
        }

        private void OnNextChessableMove(Move move)
        {
            string previousFen = ChessBoard.GetFen();
            Piece.ColorType pieceColor = Piece.GetColorFromChessboard(ChessBoard);
            if (move.MoveNotation == "Ba4")
            {

            }

            string moveNotationCoordinates = BoardHelper.GetMoveNotationCoordinates(ChessBoard, move.MoveNotation, pieceColor);
            ChessBoard.MakeMove(moveNotationCoordinates);
            MadeChessableMove?.Invoke(move);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Chessable, move.Color, move.MoveNotation, previousFen, move.Fen);
        }

        private void OnNextLichessMove(string fen, ExplorerMove move)
        {
            string previousFen = ChessBoard.GetFen();
            ChessBoard.MakeMove(move.MoveNotationCoordinates);
            MadeLichessMove?.Invoke(fen, move);
            Piece.ColorType color = Piece.GetColorFromChessboard(ChessBoard);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Lichess, color, move.MoveNotation, previousFen, fen);
        }

        public void OnPreviousMove(MoveHistory.Move historyMove)
        {
            ChessBoard.AddPiecesFromFen(historyMove.fen);
        }
    }
}
