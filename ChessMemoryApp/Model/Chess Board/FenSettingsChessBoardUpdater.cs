using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.ChessMoveLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    /// <summary>
    /// Updates fen settings after a move has been made
    /// </summary>
    public class FenSettingsChessBoardUpdater : FenSettingsUpdater
    {
        public delegate void UpdatedFenEventHandler(string fen);
        public event UpdatedFenEventHandler UpdatedFen;

        private readonly ChessboardGenerator chessboard;

        public FenSettingsChessBoardUpdater(ChessboardGenerator chessboard) : base(chessboard.fenSettings)
        {
            this.chessboard = chessboard;
        }

        public void SubscribeToEvents(PieceMover pieceMover, MoveHistory moveHistory)
        {
            pieceMover.MadeLichessMove += OnMadeLichessMove;
            pieceMover.MadeChessableMove += OnMadeChessableMove;

            moveHistory.RequestingPreviousMove += OnRequestingPreviousMove;
            moveHistory.RequestingFirstMove += OnRequestingFirstMove;
        }

        private void OnRequestingFirstMove(MoveHistory.Move firstMove)
        {
            chessboard.fenSettings = firstMove.fenSettings;
            UpdatedFen?.Invoke(firstMove.fen + " " + firstMove.fenSettings.AppliedFenSettings);
        }

        private void OnRequestingPreviousMove(MoveHistory.Move currentMove, MoveHistory.Move previousMove)
        {
            chessboard.fenSettings = previousMove.fenSettings;
            UpdatedFen?.Invoke(chessboard.currentFen);
        }

        private void OnMadeChessableMove(Move move)
        {
            string fenColor = chessboard.playAsBlack ? FenSettings.FenColor.WHITE : FenSettings.FenColor.BLACK;
            chessboard.fenSettings.SetColorToPlay(fenColor);

            UpdateFenCastleSettings(move.MoveNotation, FenSettings.FenColor.GetPieceColor(fenColor));
            UpdatePawnPlyCount(move.MoveNotation);
            UpdateEnPassant(move.MoveNotation, chessboard.currentFen);
            UpdatedFen?.Invoke(chessboard.currentFen);
        }

        private void OnMadeLichessMove(string fen, ExplorerMove move)
        {
            string fenColor = chessboard.playAsBlack ? FenSettings.FenColor.BLACK : FenSettings.FenColor.WHITE;

            chessboard.fenSettings.SetColorToPlay(fenColor);
            UpdateFenCastleSettings(move.MoveNotation, FenSettings.FenColor.GetPieceColor(fenColor));
            UpdatePawnPlyCount(move.MoveNotation);
            UpdateEnPassant(move.MoveNotation, fen);
            chessboard.fenSettings.IncreaseMoveCount();
            UpdatedFen?.Invoke(chessboard.currentFen);
        }

        private void UpdateFenSettingsOnRookMove(Piece.ColorType color)
        {
            char rank = color == Piece.ColorType.White ? '1' : '8';

            char? king = FenHelper.GetPieceOnSquare(chessboard.currentFen, "e" + rank);
            char? kingSideRook = FenHelper.GetPieceOnSquare(chessboard.currentFen, "h" + rank);
            char? queenSideRook = FenHelper.GetPieceOnSquare(chessboard.currentFen, "a" + rank);

            bool movedKingSideRook = king.HasValue && !kingSideRook.HasValue;
            bool movedQueenSideRook = king.HasValue && !queenSideRook.HasValue;

            if (movedKingSideRook)
            {
                if (color == Piece.ColorType.White)
                    fenSettings.DisableWhiteKingSideCastling();
                else
                    fenSettings.DisableBlackKingSideCastling();
            }
            if (movedQueenSideRook)
            {
                if (color == Piece.ColorType.White)
                    fenSettings.DisableWhiteQueenSideCastling();
                else
                    fenSettings.DisableBlackQueenSideCastling();
            }
        }

        protected void UpdateFenCastleSettings(string moveNotation, Piece.ColorType color)
        {
            if (moveNotation == "O-O" || moveNotation == "O-O-O")
            {
                if (fenSettings.GetColorToPlaySetting().Equals(FenSettings.FenColor.BLACK))
                {
                    fenSettings.DisableWhiteKingSideCastling();
                    fenSettings.DisableWhiteQueenSideCastling();
                }
                else
                {
                    fenSettings.DisableBlackKingSideCastling();
                    fenSettings.DisableBlackQueenSideCastling();
                }
            }
            else if (moveNotation[0] == 'R')
                UpdateFenSettingsOnRookMove(Piece.GetOppositeColor(color));
        }
    }
}
