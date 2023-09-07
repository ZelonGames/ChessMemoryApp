using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.Model.Variations;
using ChessMemoryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class CourseChessboard : ChessboardGenerator
    {
        public delegate void CourseChessBoardHandler(Course course);
        public delegate void ChessableClickHandler(string fen);
        public event CourseChessBoardHandler Clicked;

        public readonly Course course;
        protected readonly TapGestureRecognizer tapGestureRecognizer;
        protected readonly UITitleChessBoard uiTitleChessBoard;

        public string fen;

        // TODO: Fix playAsBlack
        public CourseChessboard(Course course, AbsoluteLayout chessBoardLayout, Size size, bool playAsBlack) :
            base(chessBoardLayout, size, playAsBlack)
        {
            this.course = course;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            uiTitleChessBoard = new UITitleChessBoard(size, chessBoardLayout, course != null ? course.Name : "Add new");
            uiTitleChessBoard.AddGestureRecognizers(tapGestureRecognizer, OnBoardEntered, OnBoardExited);
        }

        public CourseChessboard(Course course, AbsoluteLayout chessBoardLayout, Size size, string title, bool playAsBlack) :
            base(chessBoardLayout, size, playAsBlack)
        {
            this.course = course;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            uiTitleChessBoard = new UITitleChessBoard(size, chessBoardLayout, title);
            uiTitleChessBoard.AddGestureRecognizers(tapGestureRecognizer, OnBoardEntered, OnBoardExited);
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            Clicked?.Invoke(course);
        }


        public virtual void OnBoardEntered(object sender, EventArgs e)
        {
            uiTitleChessBoard.HideUI();
        }

        public virtual void OnBoardExited(object sender, EventArgs e)
        {
            uiTitleChessBoard.ShowUI();
        }

        public void HideUI()
        {
            uiTitleChessBoard.ClearGestureRecognizers();
            uiTitleChessBoard.HideUI();
        }

        public async void OnBoardClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MainPage");
        }

        public override void UpdateSquaresBounds()
        {
            base.UpdateSquaresBounds();
            uiTitleChessBoard.UpdateUIPosition(BoardSize, offset);
        }
    }
}
