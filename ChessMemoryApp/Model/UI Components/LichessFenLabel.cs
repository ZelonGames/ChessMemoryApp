using ChessMemoryApp.Model.BoardState;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class LichessFenLabel
    {
        private readonly Label label;
        private readonly ChessboardGenerator chessBoard;
        private string url;

        public LichessFenLabel(Label label, ChessboardGenerator chessBoard)
        {
            this.label = label;
            this.chessBoard = chessBoard;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            label.GestureRecognizers.Add(tapGestureRecognizer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribers">FenSettingsUpdater</param>
        public void SubscribeToEvents(FenSettingsChessBoardUpdater fenSettingsUpdater)
        {
            fenSettingsUpdater.UpdatedFen += OnUpdatedFen;
        }

        public void OnUpdatedFen(string fen)
        {
            url = FenHelper.ConvertFenToLichessUrl(fen, chessBoard.fenSettings, Piece.GetOppositeColor(chessBoard.boardColorOrientation));
            label.Text = fen.Split(' ')[0] + chessBoard.fenSettings.GetAppliedSettings(FenSettings.SpaceEncoding.SPACE);
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
