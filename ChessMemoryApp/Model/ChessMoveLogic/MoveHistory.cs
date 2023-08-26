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
    public class MoveHistory : IEventController
    {
        public enum MoveSource
        {
            Lichess,
            Chessable
        }

        public delegate void PreviousMoveHandler(Move currentMove, Move previousMove);

        public event PreviousMoveHandler RequestingPreviousMove;
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

            CourseMaker.Move move = courseMoveNavigator.GetRelativeMove(Course.MoveNavigation.Current, chessBoard.currentFen);
            historyMoves.Add(new Move(
                MoveSource.Chessable,
                chessBoard.fenSettings,
                move.Color,
                move.MoveNotation,
                move.MoveNotation,
                move.Fen));
            historyFens.Add(move.Fen.Split(' ')[0]);
        }

        public void SubscribeToEvents(params object[] subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                if (subscriber is PieceMover)
                {
                    var pieceMover = subscriber as PieceMover;
                    pieceMover.MadeMoveFen += OnMadeMoveFen;
                    RequestedPreviousMove += pieceMover.OnPreviousMove;
                }
                else if (subscriber is Button)
                {
                    var buttonPrevious = subscriber as Button;
                    if (buttonPrevious.Text == "<")
                        buttonPrevious.Clicked += OnReqeustedPreviousMove;
                    else if (buttonPrevious.Text == "<<")
                        buttonPrevious.Clicked += OnRequestedFirstMove;
                }
            }
        }

        private void OnRequestedFirstMove(object sender, EventArgs e)
        {
            if (historyMoves.Count <= 1)
                return;

            RequestingPreviousMove?.Invoke(historyMoves.Last(), historyMoves.First());
            historyMoves.RemoveRange(1, historyMoves.Count - 1);
            previousMove = historyMoves.First();
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

        private void OnMadeMoveFen(MoveSource moveSource, Piece.ColorType color, string moveNotation, string previousFen, string currentFen)
        {
            if (chessBoard.playAsBlack)
                color = moveSource == MoveSource.Chessable ? Piece.ColorType.Black : Piece.ColorType.White;
            else
                color = moveSource == MoveSource.Chessable ? Piece.ColorType.White : Piece.ColorType.Black;

            string moveNotationCoordinates = FenHelper.ConvertToMoveNotationCoordinates(previousFen, moveNotation);
            /*
            if (moveNotation == "O-O")
            {
                if (color == Piece.ColorType.White)
                    moveNotationCoordinates = "e1g1";
                else
                    moveNotationCoordinates = "e8g8";
            }
            else if (moveNotation == "O-O-O")
            {
                if (color == Piece.ColorType.White)
                    moveNotationCoordinates = "e1c1";
                else
                    moveNotationCoordinates = "e8c8";
            }
            else
                moveNotationCoordinates = FenHelper.GetMoveNotationCoordinates(previousFen, currentFen, color);*/
            historyMoves.Add(new Move(moveSource, chessBoard.fenSettings, color, moveNotationCoordinates, moveNotation, currentFen));
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
        }
    }
}
