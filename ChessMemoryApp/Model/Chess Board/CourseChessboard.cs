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
        protected readonly UICourseChessBoard uiCourseChessBoard;

        public string fen;

        public CourseChessboard(Course course, AbsoluteLayout chessBoardLayout, Size size) :
            base(chessBoardLayout, size)
        {
            this.course = course;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            uiCourseChessBoard = new UICourseChessBoard(size, chessBoardLayout, course != null ? course.Name : "Add new");
            uiCourseChessBoard.AddGestureRecognizers(tapGestureRecognizer, OnBoardEntered, OnBoardExited);
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            Clicked?.Invoke(course);
        }


        public virtual void OnBoardEntered(object sender, EventArgs e)
        {
            uiCourseChessBoard.HideUI();
        }

        public virtual void OnBoardExited(object sender, EventArgs e)
        {
            uiCourseChessBoard.ShowUI();
        }

        public void HideUI()
        {
            uiCourseChessBoard.ClearGestureRecognizers();
            uiCourseChessBoard.HideUI();
        }

        public async void OnBoardClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MainPage");
        }

        public override void UpdateSquaresBounds()
        {
            base.UpdateSquaresBounds();
            uiCourseChessBoard.UpdateUIPosition(BoardSize, offset);
        }
    }
}
