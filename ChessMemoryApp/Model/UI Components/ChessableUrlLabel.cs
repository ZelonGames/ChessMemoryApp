using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class ChessableUrlLabel
    {
        public readonly Course course;
        private string url;

        public ChessableUrlLabel(Label label, ChessboardGenerator chessboard, Course course)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            label.GestureRecognizers.Add(tapGestureRecognizer);
            chessboard.Loaded += ChessBoard_Loaded;

            this.course = course;
        }

        private void ChessBoard_Loaded(string fen)
        {
            url = FenHelper.ConvertFenToChessableUrl(fen, course.chessableCourseID.ToString());
        }


        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Launcher.OpenAsync(url);
            }
            catch
            {
            }
        }
    }
}
