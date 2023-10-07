using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    /// <summary>
    /// Keeps track of all the moves that has been made and the state of the board of every move
    /// </summary>
    public class MoveHistory
    {
        public enum MoveSource
        {
            Lichess,
            Chessable
        }

        public delegate void PreviousMoveHandler(Move currentMove, Move previousMove);
        public delegate void FirstMoveHandler(Move firstMove);

        public event PreviousMoveHandler RequestingPreviousMove;
        public event FirstMoveHandler RequestingFirstMove;
        public event Action<Move> RequestedPreviousMove;
        public event Action<Move> AddedMove;

        private readonly List<Move> historyMoves = new();
        private readonly HashSet<string> historyFens = new();
        private readonly ChessboardGenerator chessBoard;
        private Move previousMove;

        public MoveHistory(ChessboardGenerator chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        public void AddFirstHistoryMove(CourseMoveNavigator courseMoveNavigator)
        {
            if (historyMoves.Count > 0)
                return;

            CourseMaker.Move move = courseMoveNavigator.course.GetRelativeMove(chessBoard.GetPositionFen(), Course.MoveNavigation.Current);
            historyMoves.Add(new Move(
                MoveSource.Chessable,
                chessBoard.fenSettings,
                move.Color,
                move.MoveNotation,
                move.MoveNotation,
                move.Fen));
            historyFens.Add(move.Fen.Split(' ')[0]);
        }

        public void SubscribeToEvents(PieceMoverAuto pieceMover, Button buttonStart, Button buttonPrevious)
        {
            pieceMover.MadeMoveFen += OnMadeMoveFen;
            RequestedPreviousMove += pieceMover.OnPreviousMove;

            buttonPrevious.Clicked += OnReqeustedPreviousMove;
            buttonStart.Clicked += OnRequestedFirstMove;
        }

        private void OnRequestedFirstMove(object sender, EventArgs e)
        {
            if (historyMoves.Count <= 1)
                return;

            Move firstMove = historyMoves.First();
            RequestingFirstMove?.Invoke(firstMove);
            previousMove = firstMove;
            historyMoves.Clear();
            historyMoves.Add(firstMove);
            RequestedPreviousMove?.Invoke(previousMove);
        }

        public void OnReqeustedPreviousMove(object sender, EventArgs args)
        {
            if (historyMoves.Count <= 1)
                return;

            RequestingPreviousMove?.Invoke(historyMoves.Last(), historyMoves[historyMoves.Count - 2]);
            historyMoves.RemoveAt(historyMoves.Count - 1);
            previousMove = historyMoves.Last();
            RequestedPreviousMove?.Invoke(previousMove);
        }

        private void OnMadeMoveFen(MoveSource moveSource, Piece.ColorType color, string moveNotation, string moveNotationCoordinates, string currentFen)
        {
            if (chessBoard.boardColorOrientation == Piece.ColorType.Black)
                color = moveSource == MoveSource.Chessable ? Piece.ColorType.Black : Piece.ColorType.White;
            else
                color = moveSource == MoveSource.Chessable ? Piece.ColorType.White : Piece.ColorType.Black;
            
            historyMoves.Add(new Move(moveSource, chessBoard.fenSettings.Copy(), color, moveNotationCoordinates, moveNotation, currentFen));
            AddedMove?.Invoke(historyMoves.Last());
        }

        public void AddMove(Move move)
        {
            historyMoves.Add(move);
            AddedMove?.Invoke(historyMoves.Last());
        }

        public class Move
        {
            public readonly MoveSource moveSource;
            public readonly FenSettings fenSettings;
            public readonly Piece.ColorType color;
            public readonly string moveNotationCoordinates;
            public readonly string moveNotation;
            public readonly string fen;

            public Move(MoveSource moveSource, FenSettings fenSettings, Piece.ColorType color, string moveNotationCoordinates, string moveNotation, string fen)
            {
                this.moveSource = moveSource;
                this.fenSettings = fenSettings;
                this.color = color;
                this.moveNotationCoordinates = moveNotationCoordinates;
                this.moveNotation = moveNotation;
                this.fen = fen;
            }

            public bool IsSameMoveNotationCoordinates(string moveNotationCoordinates)
            {
                if (this.moveNotationCoordinates is "e1h1" or "e1g1" && moveNotationCoordinates is "e1h1" or "e1g1" ||
                    this.moveNotationCoordinates is "e8h8" or "e8g8" && moveNotationCoordinates is "e8h8" or "e8g8")
                    return true;

                if (this.moveNotationCoordinates is "e1a1" or "e1c1" && moveNotationCoordinates is "e1c1" or "e1c1" ||
                    this.moveNotationCoordinates is "e8a8" or "e8c8" && moveNotationCoordinates is "e8c8" or "e8c8")
                    return true;

                return false;
            }
        }
    }
}
