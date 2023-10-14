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
    public static class ChessableUrlLabel
    {
        private static Course course;
        private static string url;

        public static void Install(Label label, UIChessBoard chessboard, Course course)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            label.GestureRecognizers.Add(tapGestureRecognizer);
            chessboard.ChangedPieces += OnChangedPieces;
            //fenSettingsUpdater.UpdatedFen += OnUpdatedFen;   

            ChessableUrlLabel.course = course;
        }

        private static void OnChangedPieces(string fen)
        {
            url = FenHelper.ConvertFenToChessableUrl(fen, course.chessableCourseID.ToString());
        }

        private static async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
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
