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
    /// Gets the previous or next move from the selected course
    /// </summary>
    public class CourseMoveNavigator
    {
        public delegate void CourseMoveNavigatorEventHandler(Move move);
        public event CourseMoveNavigatorEventHandler RequestedNextChessableMove;

        public delegate void PreviousMoveEventHandler(string fen);
        public event PreviousMoveEventHandler RequestedPreviousMove;

        private readonly ChessboardGenerator chessboard;
        private readonly VariationLoader variationLoader;
        private readonly Course course;

        public CourseMoveNavigator(ChessboardGenerator chessboard, VariationLoader variationLoader, Course course)
        {
            this.chessboard = chessboard;
            this.variationLoader = variationLoader;
            this.course = course;
        }

        public void OnButtonNextClicked(object sender, EventArgs args)
        {
            if (variationLoader.IsLoadingLichess)
                return;

            if (variationLoader.lichessButtons.Count > 0)
            {
                // Simulate that you are clicking on the first lichess button
                variationLoader.lichessButtons.First().OnClicked(null, null);
                return;
            }

            Move move = GetRelativeMove(Course.MoveNavigation.Next, chessboard.GetFen());
            if (move != null)
                RequestedNextChessableMove?.Invoke(move);
        }

        public Move GetRelativeMove(Course.MoveNavigation moveNavigation, string fen)
        {
            fen = fen.Split(' ')[0];
            return course.GetRelativeMove(fen, moveNavigation);
        }

        public void SubscribeToEvents(Button buttonNext)
        {
            buttonNext.Clicked += OnButtonNextClicked;
        }
    }
}
