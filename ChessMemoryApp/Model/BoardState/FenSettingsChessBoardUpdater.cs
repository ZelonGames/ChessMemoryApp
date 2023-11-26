using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.ChessMoveLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;

namespace ChessMemoryApp.Model.BoardState
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

        /// <summary>
        /// Updates the fen settings whenever chessboard.MovedPiece() was called
        /// </summary>
        public void UseChessBoardMovedPiece()
        {
            chessboard.MovedPiece += OnMovedPiece;
        }

        /// <summary>
        /// Updates the fen settings when a move from either lichess or chessable was made
        /// </summary>
        /// <param name="pieceMover"></param>
        /// <param name="moveHistory"></param>
        public void SubscribeToEvents(PieceMoverAuto pieceMover, MoveHistory moveHistory)
        {
            pieceMover.MadeLichessMove += OnMadeLichessMove;
            pieceMover.MadeChessableMove += OnMadeChessableMove;
            moveHistory.RequestingPreviousMove += OnRequestingPreviousMove;
            moveHistory.RequestingFirstMove += OnRequestingFirstMove;
        }

        private void OnMovedPiece(MovedPieceData movedPieceData)
        {
            if (!movedPieceData.updateBoardState)
                return;

            string moveNotationCoordinates = movedPieceData.fromCoordinates + movedPieceData.toCoordinates;

            UpdateFenCastleSettings(moveNotationCoordinates, chessboard.boardColorOrientation);
            UpdateEnPassant(moveNotationCoordinates);
            fenSettings.IncreaseMoveCount();
        }

        private void OnRequestingFirstMove(MoveHistory.Move firstMove)
        {
            chessboard.fenSettings = fenSettings = firstMove.fenSettings.Copy();
            UpdatedFen?.Invoke(firstMove.fen + " " + firstMove.fenSettings.AppliedFenSettings);
        }

        private void OnRequestingPreviousMove(MoveHistory.Move currentMove, MoveHistory.Move previousMove)
        {
            chessboard.fenSettings = fenSettings = previousMove.fenSettings.Copy();
            UpdatedFen?.Invoke(chessboard.GetPositionFen());
        }

        private void OnMadeChessableMove(string moveNotationCoordinates, Move move)
        {
            Piece.ColorType fenColor = Piece.GetOppositeColor(chessboard.boardColorOrientation);
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromPieceColor(fenColor));
            string fen = chessboard.GetPositionFen();
            UpdateFenCastleSettings(moveNotationCoordinates, chessboard.boardColorOrientation);
            UpdatePawnPlyCount(move.MoveNotation);
            UpdateEnPassant(move.MoveNotation, fen);
            UpdatedFen?.Invoke(fen);
        }

        private void OnMadeLichessMove(string fen, ExplorerMove move)
        {
            string chessBoardFen = chessboard.GetPositionFen();
            UpdateFenCastleSettings(move.MoveNotationCoordinates, Piece.GetOppositeColor(chessboard.boardColorOrientation));
            UpdatePawnPlyCount(move.MoveNotation);
            UpdateEnPassant(move.MoveNotation, fen);
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromPieceColor(chessboard.boardColorOrientation));
            fenSettings.IncreaseMoveCount();
            UpdatedFen?.Invoke(chessBoardFen);
        }

        private void UpdateEnPassant(string lastMoveNotationCoordinates)
        {
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(lastMoveNotationCoordinates);
            string toCoordinates = BoardHelper.GetToCoordinatesString(lastMoveNotationCoordinates);
            Piece.Coordinates<int> fromNumberCoordinates = BoardHelper.GetNumberCoordinates(fromCoordinates);
            Piece.Coordinates<int> toNumberCoordinates = BoardHelper.GetNumberCoordinates(toCoordinates);

            Piece movedPiece = chessboard.GetPiece(toCoordinates);
            bool isDoublePawnMove = Math.Abs(fromNumberCoordinates.Y - toNumberCoordinates.Y) == 2;

            if (movedPiece is not Pawn || !isDoublePawnMove)
            {
                fenSettings.DisableEnPassant();
                return;
            }

            var leftSideCoordinates = new Piece.Coordinates<int>(toNumberCoordinates.X - 1, toNumberCoordinates.Y);
            Piece leftSidePiece = chessboard.GetPiece(BoardHelper.GetLetterCoordinates(leftSideCoordinates));
            var enPassantCoordinates = new Piece.Coordinates<int>();
            enPassantCoordinates.Y = toNumberCoordinates.Y + (movedPiece.color == Piece.ColorType.White ? -1 : 1);

            if (leftSidePiece != null && leftSidePiece is Pawn)
            {
                enPassantCoordinates.X = toNumberCoordinates.X - 1;
                fenSettings.SetEnPassantSquare(BoardHelper.GetLetterCoordinates(enPassantCoordinates));
                return;
            }

            var rightSideCoordinates = new Piece.Coordinates<int>(toNumberCoordinates.X + 1, toNumberCoordinates.Y);
            Piece rightSidePiece = chessboard.GetPiece(BoardHelper.GetLetterCoordinates(rightSideCoordinates));

            if (rightSidePiece != null && rightSidePiece is Pawn)
            {
                enPassantCoordinates.X = toNumberCoordinates.X + 1;
                fenSettings.SetEnPassantSquare(BoardHelper.GetLetterCoordinates(enPassantCoordinates));
                return;
            }

            fenSettings.DisableEnPassant();
        }

        private void UpdateCastlingSettingsOnRookMove(Piece.ColorType color)
        {
            string fen = chessboard.GetPositionFen();
            char rank = color == Piece.ColorType.White ? '1' : '8';
            char? king = FenHelper.GetPieceOnSquare(fen, "e" + rank);
            char? kingSideRook = FenHelper.GetPieceOnSquare(fen, "h" + rank);
            char? queenSideRook = FenHelper.GetPieceOnSquare(fen, "a" + rank);
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

        private void UpdateFenCastleSettings(string moveNotationCoordinates, Piece.ColorType color)
        {
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveNotationCoordinates);
            Piece piece = chessboard.GetPiece(toCoordinates);

            bool isCastlingMove = moveNotationCoordinates is
                "e1g1" or "e1h1" or "e1c1" or "e1a1" or
                "e8g8" or "e8h8" or "e8c8" or "e8a8";
            bool movedKing = piece != null && piece is King;
            bool movedRook = piece != null && piece is Rook;

            if (isCastlingMove || movedKing)
            {
                if (color == Piece.ColorType.White)
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
            else if (movedRook)
                UpdateCastlingSettingsOnRookMove(color);
        }
    }
}
