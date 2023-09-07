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
            ChessBoard.LoadChessBoardFromFen(fen);
        }

        public void SubscribeToEvents(CourseMoveNavigator courseMoveNavigator)
        {
            LichessButton.RequestedNewFen += OnNextLichessMove;
            courseMoveNavigator.RequestedNextChessableMove += OnNextChessableMove;
        }

        private void CustomVariationMoveNavigator_GuessedCorrectMove(MoveHistory.Move moveToMake)
        {
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveToMake.moveNotationCoordinates);
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveToMake.moveNotationCoordinates);
            string fen = ChessBoard.GetFen();
            bool madeCastleMove = TryMakeCastleMove(fromCoordinates, toCoordinates);

            if (!madeCastleMove)
                ChessBoard.MovePiece(fromCoordinates, toCoordinates);

            MovedPiece?.Invoke(fen);
            moveNotationHelper.ResetClicks();
        }

        private void OnNextChessableMove(Move move)
        {
            string previousFen = ChessBoard.GetFen();
            ChessBoard.LoadChessBoardFromFen(move.Fen);
            MadeChessableMove?.Invoke(move);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Chessable, move.Color, move.MoveNotation, previousFen, move.Fen);
        }

        private void OnNextLichessMove(string fen, ExplorerMove move)
        {
            string previousFen = ChessBoard.GetFen();
            ChessBoard.LoadChessBoardFromFen(fen);
            MadeLichessMove?.Invoke(fen, move);
            Piece.ColorType color = Piece.GetColorFromChessboard(ChessBoard);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Lichess, color, move.MoveNotation, previousFen, fen);
        }

        public void OnPreviousMove(MoveHistory.Move historyMove)
        {
            ChessBoard.LoadChessBoardFromFen(historyMove.fen);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="fromCoordinates"></param>
        /// <param name="toCoordinates"></param>
        /// <returns>returns true if the move was made</returns>
        private bool TryMakeCastleMove(string fromCoordinates, string toCoordinates)
        {
            Piece piece = ChessBoard.GetPiece(fromCoordinates);
            if (piece == null || piece != null && piece.pieceType != 'k' && piece.pieceType != 'K')
                return false;

            string moveNotationCoordinates = fromCoordinates + toCoordinates;

            // Short Castle White
            Piece rook = ChessBoard.GetPiece("h1");
            if (rook != null && rook.color == Piece.ColorType.White && moveNotationCoordinates == "e1g1")
            {
                ChessBoard.MovePiece(fromCoordinates, toCoordinates);
                ChessBoard.MovePiece("h1", "f1");
                return true;
            }

            // Long Castle White
            rook = ChessBoard.GetPiece("a1");
            if (rook != null && rook.color == Piece.ColorType.White && moveNotationCoordinates == "e1c1")
            {
                ChessBoard.MovePiece(fromCoordinates, toCoordinates);
                ChessBoard.MovePiece("a1", "d1");
                return true;
            }

            // Short Castle Black
            rook = ChessBoard.GetPiece("h8");
            if (rook != null && rook.color == Piece.ColorType.Black && moveNotationCoordinates == "e8g8")
            {
                ChessBoard.MovePiece(fromCoordinates, toCoordinates);
                ChessBoard.MovePiece("h8", "f8");
                return true;
            }

            // Long Castle Black
            rook = ChessBoard.GetPiece("a8");
            if (rook != null && rook.color == Piece.ColorType.Black && moveNotationCoordinates == "e8c8")
            {
                ChessBoard.MovePiece(fromCoordinates, toCoordinates);
                ChessBoard.MovePiece("a8", "d8");
                return true;
            }

            return false;
        }
    }
}
