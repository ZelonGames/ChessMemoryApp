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
    /// Moves the pieces when a move from Lichess or Chessable was made
    /// </summary>
    public class PieceMoverAuto
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

        private readonly ChessboardGenerator chessBoard;

        public PieceMoverAuto(ChessboardGenerator chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        public void SubscribeToEvents(CourseMoveNavigator courseMoveNavigator)
        {
            LichessButton.Clicked += OnNextLichessMove;
            courseMoveNavigator.RequestedNextChessableMove += OnNextChessableMove;
        }

        private void OnNextChessableMove(Move move)
        {
            string previousFen = chessBoard.GetPositionFen();
            Piece.ColorType pieceColor = Piece.GetColorFromChessboard(chessBoard);
            string moveNotationCoordinates = BoardHelper.GetMoveNotationCoordinates(chessBoard, move.MoveNotation, pieceColor);

            chessBoard.MakeMove(moveNotationCoordinates);
            MadeChessableMove?.Invoke(move);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Chessable, move.Color, move.MoveNotation, previousFen, move.Fen);
        }

        private void OnNextLichessMove(string fen, ExplorerMove move)
        {
            string previousFen = chessBoard.GetPositionFen();
            chessBoard.MakeMove(move.MoveNotationCoordinates);
            MadeLichessMove?.Invoke(fen, move);
            Piece.ColorType color = Piece.GetColorFromChessboard(chessBoard);
            MadeMoveFen?.Invoke(MoveHistory.MoveSource.Lichess, color, move.MoveNotation, previousFen, fen);
        }

        public void OnPreviousMove(MoveHistory.Move historyMove)
        {
            chessBoard.AddPiecesFromFen(historyMove.fen);
        }
    }
}
