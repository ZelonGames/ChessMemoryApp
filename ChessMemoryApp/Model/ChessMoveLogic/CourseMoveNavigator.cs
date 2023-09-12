using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.ChessMoveLogic
{
    /// <summary>
    /// Gets the previous or next move from the selected course or from Lichess
    /// </summary>
    public class CourseMoveNavigator
    {
        public delegate void CourseMoveNavigatorEventHandler(Move move);
        public event CourseMoveNavigatorEventHandler RequestedNextChessableMove;

        public delegate void PreviousMoveEventHandler(string fen);
        public event PreviousMoveEventHandler RequestedPreviousMove;

        private readonly ChessboardGenerator chessboard;
        private readonly LichessMoveLoader lichessMoveLoader;
        public readonly Course course;

        public CourseMoveNavigator(ChessboardGenerator chessboard, LichessMoveLoader lichessMoveLoader, Course course)
        {
            this.chessboard = chessboard;
            this.lichessMoveLoader = lichessMoveLoader;
            this.course = course;
        }

        public void OnButtonNextClicked(object sender, EventArgs args)
        {
            if (lichessMoveLoader.IsLoadingLichess)
                return;

            if (lichessMoveLoader.lichessButtons.Count > 0)
            {
                // Simulate that you are clicking on the first lichess button
                lichessMoveLoader.lichessButtons.First().OnClicked(null, null);
            }
            else
            {
                Move move = course.GetRelativeMove(chessboard.GetPositionFen(), Course.MoveNavigation.Next);
                if (move != null)
                    RequestedNextChessableMove?.Invoke(move);
            }
        }

        public void SubscribeToEvents(Button buttonNext)
        {
            buttonNext.Clicked += OnButtonNextClicked;
        }
    }
}
