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
    public class FenSettingsUpdater : IEventController
    {
        public delegate void UpdatedFenEventHandler(string fen);
        public event UpdatedFenEventHandler UpdatedFen;

        private readonly ChessboardGenerator chessboard;

        public FenSettingsUpdater(ChessboardGenerator chessboard)
        {
            this.chessboard = chessboard;

        }

        public void SubscribeToEvents(params object[] subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                if (subscriber is PieceMover)
                {
                    var pieceMover = subscriber as PieceMover;
                    pieceMover.MadeLichessMove += OnMadeLichessMove;
                    pieceMover.MadeChessableMove += OnMadeChessableMove;
                }
                else if (subscriber is MoveHistory)
                {
                    var moveHistory = subscriber as MoveHistory;
                    moveHistory.RequestingPreviousMove += OnRequestingPreviousMove;
                    moveHistory.RequestingFirstMove += OnRequestingFirstMove;
                }
            }
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
            UpdatedFen?.Invoke(chessboard.currentFen);
        }

        private void OnMadeLichessMove(string fen, ExplorerMove move)
        {
            string fenColor = chessboard.playAsBlack ? FenSettings.FenColor.BLACK : FenSettings.FenColor.WHITE;

            chessboard.fenSettings.SetColorToPlay(fenColor);
            UpdateFenCastleSettings(move.MoveNotation, FenSettings.FenColor.GetPieceColor(fenColor));
            UpdatePawnPlyCount(move.MoveNotation);
            chessboard.fenSettings.IncreaseMoveCount();
            UpdatedFen?.Invoke(chessboard.currentFen);
        }

        private void UpdatePawnPlyCount(string moveNotation)
        {
            bool isPawnMove = true;
            bool shouldReset = false;

            if (moveNotation.Contains('x'))
                shouldReset = true;
            else
            {
                foreach (var character in moveNotation.Split('x')[0])
                {
                    if (char.IsUpper(character))
                    {
                        isPawnMove = false;
                        break;
                    }
                }

                shouldReset = isPawnMove;
            }

            if (shouldReset)
                chessboard.fenSettings.ResetPlyCount();
            else
                chessboard.fenSettings.IncreasePlyCount();
        }

        private void UpdateFenCastleSettings(string moveNotation, Piece.ColorType color)
        {
            if (moveNotation == "O-O" || moveNotation == "O-O-O")
            {
                if (chessboard.fenSettings.GetColorToPlaySetting().Equals(FenSettings.FenColor.BLACK))
                {
                    chessboard.fenSettings.DisableWhiteKingSideCastling();
                    chessboard.fenSettings.DisableWhiteQueenSideCastling();
                }
                else
                {
                    chessboard.fenSettings.DisableBlackKingSideCastling();
                    chessboard.fenSettings.DisableBlackQueenSideCastling();
                }
            }
            else if (moveNotation[0] == 'R')
                UpdateFenSettingsOnRookMove(moveNotation, Piece.GetOppositeColor(color));
        }

        private void UpdateFenSettingsOnRookMove(string moveNotation, Piece.ColorType color)
        {
            char rank = color == Piece.ColorType.White ? '1' : '8';

            char? king = FenHelper.GetPieceOnSquare(chessboard.currentFen, "e" + rank);
            char? kingSideRook = FenHelper.GetPieceOnSquare(chessboard.currentFen, "h" + rank);
            char? queenSideRook = FenHelper.GetPieceOnSquare(chessboard.currentFen, "a" + rank);

            Piece.Coordinates<char> toCoordinates = BoardHelper.GetCoordinates(moveNotation);

            bool movedKingSideRook = king.HasValue && !kingSideRook.HasValue;
            bool movedQueenSideRook = king.HasValue && !queenSideRook.HasValue;

            if (movedKingSideRook)
            {
                if (color == Piece.ColorType.White)
                    chessboard.fenSettings.DisableWhiteKingSideCastling();
                else
                    chessboard.fenSettings.DisableBlackKingSideCastling();
            }
            if (movedQueenSideRook)
            {
                if (color == Piece.ColorType.White)
                    chessboard.fenSettings.DisableWhiteQueenSideCastling();
                else
                    chessboard.fenSettings.DisableBlackQueenSideCastling();
            }
        }
    }
}
