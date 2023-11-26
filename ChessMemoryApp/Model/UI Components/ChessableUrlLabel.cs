using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class ChessableUrlLabel
    {
        private readonly CustomVariation customVariation;
        private readonly Course course;
        private string url;

        public ChessableUrlLabel(Label label, UIChessBoard chessboard, Course course)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            label.GestureRecognizers.Add(tapGestureRecognizer);
            chessboard.ChangedPieces += OnChangedPieces;
            //fenSettingsUpdater.UpdatedFen += OnUpdatedFen;   

            this.course = course;
        }

        public ChessableUrlLabel(Label label, UIChessBoard chessboard, CustomVariation customVariation)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            label.GestureRecognizers.Add(tapGestureRecognizer);
            url = FenHelper.ConvertFenToChessableUrl(customVariation.moves[customVariation.moves.Count - 2].fen, customVariation.Course.chessableCourseID.ToString());
        }

        private void OnChangedPieces(string fen)
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
