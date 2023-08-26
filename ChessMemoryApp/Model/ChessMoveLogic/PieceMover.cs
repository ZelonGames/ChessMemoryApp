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
            GetChessboard().LoadChessBoardFromFen(fen);
        }

        public void SubscribeToEvents(CourseMoveNavigator courseMoveNavigator)
        {
            LichessButton.RequestedNewFen += OnNextLichessMove;
            courseMoveNavigator.RequestedNextChessableMove += OnNextChessableMove;
        }

        public ChessboardGenerator GetChessboard()
        {
            return moveNotationHelper.chessBoard;
        }

        private void CustomVariationMoveNavigator_GuessedCorrectMove(MoveHistory.Move moveToMake)
        {
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveToMake.moveNotationCoordinates);
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveToMake.moveNotationCoordinates);

            bool madeCastleMove; 
            TryMakeCastleMove(moveNotationHelper.chessBoard.currentFen, fromCoordinates, toCoordinates, out madeCastleMove);

            if (!madeCastleMove)
                MakeTeleportMove(fromCoordinates, toCoordinates);

            MovedPiece?.Invoke(moveNotationHelper.chessBoard.currentFen);
            moveNotationHelper.ResetClicks();
        }

        private void OnNextChessableMove(Move move)
        {
            string previousFen = GetChessboard().currentFen;
            GetChessboard().LoadChessBoardFromFen(move.Fen);
            MadeChessableMove?.Invoke(move);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Chessable, move.Color, move.MoveNotation, previousFen, move.Fen);
        }

        private void OnNextLichessMove(string fen, ExplorerMove move)
        {
            string previousFen = GetChessboard().currentFen;
            GetChessboard().LoadChessBoardFromFen(fen);
            MadeLichessMove?.Invoke(fen, move);
            Piece.ColorType color = Piece.GetColorFromChessboard(GetChessboard());
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Lichess, color, move.MoveNotation, previousFen, GetChessboard().currentFen);
        }

        public void OnPreviousMove(MoveHistory.Move historyMove)
        {
            GetChessboard().LoadChessBoardFromFen(historyMove.fen);
        }

        private void MakeTeleportMove(string fromCoordinates, string toCoordinates)
        {
            char? removedPiece; 
            string newFen = FenHelper.RemovePieceFromFEN(moveNotationHelper.chessBoard.currentFen, fromCoordinates, out removedPiece);
            if (!removedPiece.HasValue)
                return;
            newFen = FenHelper.AddPieceToFEN(newFen, toCoordinates, removedPiece.Value);
            moveNotationHelper.chessBoard.LoadChessBoardFromFen(newFen);
        }

        /// <summary>
        /// returns true if the move should and was made
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="fromCoordinates"></param>
        /// <param name="toCoordinates"></param>
        /// <returns></returns>
        private void TryMakeCastleMove(string fen, string fromCoordinates, string toCoordinates, out bool madeMove)
        {
            char? piece = FenHelper.GetPieceOnSquare(fen, fromCoordinates);
            if (!piece.HasValue || piece.HasValue && piece.Value != 'k' && piece.Value != 'K')
            {
                madeMove = false;
                return;
            }

            // Short Castle White
            char? rook = FenHelper.GetPieceOnSquare(fen, "h1");
            if (rook.HasValue && rook.Value == 'R' && fromCoordinates + toCoordinates == "e1g1")
            {
                MakeTeleportMove(fromCoordinates, toCoordinates);
                MakeTeleportMove("h1", "f1");
                madeMove = true;
                return;
            }

            // Long Castle White
            rook = FenHelper.GetPieceOnSquare(fen, "a1");
            if (rook.HasValue && rook.Value == 'R' && fromCoordinates + toCoordinates == "e1c1")
            {
                MakeTeleportMove(fromCoordinates, toCoordinates);
                MakeTeleportMove("a1", "d1");
                madeMove = true;
                return;
            }

            // Short Castle White
            rook = FenHelper.GetPieceOnSquare(fen, "h8");
            if (rook.HasValue && rook.Value == 'r' && fromCoordinates + toCoordinates == "e8g8")
            {
                MakeTeleportMove(fromCoordinates, toCoordinates);
                MakeTeleportMove("h8", "f8");
                madeMove = true;
                return;
            }

            // Long Castle White
            rook = FenHelper.GetPieceOnSquare(fen, "a8");
            if (rook.HasValue && rook.Value == 'r' && fromCoordinates + toCoordinates == "e8c8")
            {
                MakeTeleportMove(fromCoordinates, toCoordinates);
                MakeTeleportMove("a8", "d8");
                madeMove = true;
                return;
            }

            madeMove = false;
        }
    }
}
