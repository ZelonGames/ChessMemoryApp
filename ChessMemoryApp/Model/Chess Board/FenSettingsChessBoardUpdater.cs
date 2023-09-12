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
        public void SubscribeToEvents(PieceMover pieceMover, MoveHistory moveHistory)
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

        private void OnMadeChessableMove(Move move)
        {
            Piece.ColorType fenColor = Piece.GetOppositeColor(chessboard.boardColorOrientation);
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromPieceColor(fenColor));
            string fen = chessboard.GetPositionFen();
            UpdateFenCastleSettings(move.MoveNotation, chessboard.boardColorOrientation);
            UpdatePawnPlyCount(move.MoveNotation);
            UpdateEnPassant(move.MoveNotation, fen);
            UpdatedFen?.Invoke(fen);
        }

        private void OnMadeLichessMove(string fen, ExplorerMove move)
        {
            string chessBoardFen = chessboard.GetPositionFen();
            Piece.ColorType fenColor = chessboard.boardColorOrientation;
            UpdateFenCastleSettings(move.MoveNotation, Piece.GetOppositeColor(chessboard.boardColorOrientation));
            UpdatePawnPlyCount(move.MoveNotation);
            UpdateEnPassant(move.MoveNotation, fen);
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromPieceColor(fenColor));
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

        private void UpdateFenSettingsOnRookMove(Piece.ColorType color)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveNotation">Works with both algebraic and coordinates notations</param>
        /// <param name="color"></param>
        private void UpdateFenCastleSettings(string moveNotation, Piece.ColorType color)
        {
            if (moveNotation == "O-O" || moveNotation == "O-O-O" ||
                moveNotation == "e1g1" || moveNotation == "e1h1" ||
                moveNotation == "e1c1" || moveNotation == "e1a1" ||
                moveNotation == "e8g8" || moveNotation == "e8h8" ||
                moveNotation == "e8c8" || moveNotation == "e8a8")
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
            else
            {
                string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveNotation);
                Piece piece = chessboard.GetPiece(fromCoordinates);
                if (moveNotation[0] == 'R' || piece != null && piece is Rook)
                    UpdateFenSettingsOnRookMove(Piece.GetOppositeColor(color));
            }
        }
    }
}
