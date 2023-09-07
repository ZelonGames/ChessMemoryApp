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
    public class ChessableChessboard : ChessboardGenerator
    {
        public delegate void CourseChessBoardHandler(Course course);
        public delegate void ChessableClickHandler(string fen);
        public event ChessableClickHandler Clicked;

        protected readonly TapGestureRecognizer tapGestureRecognizer;
        private readonly UITitleChessBoard uiCourseChessBoard;

        public string fen;

        public ChessableChessboard(AbsoluteLayout chessBoardLayout, Size size, bool playAsBlack) :
            base(chessBoardLayout, size, playAsBlack)
        {
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            uiCourseChessBoard = new UITitleChessBoard(size, chessBoardLayout, "");
            uiCourseChessBoard.AddGestureRecognizers(tapGestureRecognizer, OnBoardEntered, OnBoardExited);
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            Clicked?.Invoke(fen);
        }


        public virtual void OnBoardEntered(object sender, EventArgs e)
        {
            uiCourseChessBoard.HideUI();
        }

        public virtual void OnBoardExited(object sender, EventArgs e)
        {
            uiCourseChessBoard.ShowUI();
        }

        public override void UpdateSquaresBounds()
        {
            base.UpdateSquaresBounds();
            uiCourseChessBoard.UpdateUIPosition(BoardSize, offset);
        }
    }
}
