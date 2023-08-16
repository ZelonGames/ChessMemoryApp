using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class LichessFenLabel : IEventController
    {
        private readonly Label label;
        private readonly ChessboardGenerator chessboard;
        private string url;

        public LichessFenLabel(Label label, ChessboardGenerator chessboard)
        {
            this.label = label;
            this.chessboard = chessboard;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            label.GestureRecognizers.Add(tapGestureRecognizer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribers">FenSettingsUpdater</param>
        public void SubscribeToEvents(params object[] subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                if (subscriber is FenSettingsUpdater fenSettingsUpdater)
                    fenSettingsUpdater.UpdatedFen += FenSettingsUpdater_UpdatedFen;
            }
        }

        public void FenSettingsUpdater_UpdatedFen(string fen)
        {
            url = FenHelper.ConvertFenToLichessUrl(fen, chessboard.fenSettings, !chessboard.playAsBlack);
            label.Text = fen.Split(' ')[0] + FenSettings.SpaceEncoding.SPACE + chessboard.fenSettings.GetAppliedSettings(FenSettings.SpaceEncoding.SPACE);
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
