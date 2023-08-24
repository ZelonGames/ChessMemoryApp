using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ChessMemoryApp.Model.ChessMoveLogic;
using SQLite;
using ChessMemoryApp.Services;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Lichess.Lichess_API;

namespace ChessMemoryApp.Model.Variations
{
    public class CustomVariation : IEventController, ISqLiteService_CustomVariation
    {
        [JsonIgnore, PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [JsonIgnore]
        public string CourseName { get; set; }

        [JsonIgnore]
        public string JsonData { get; set; }

        [JsonProperty("course")]
        public Course Course { get; private set; }

        [JsonProperty("moves")]
        public readonly List<MoveHistory.Move> moves = new();

        [JsonProperty("previewFen")]
        public string PreviewFen { get; private set; }

        [JsonIgnore]
        private VerticalStackLayout verticalStackLayout;

        [JsonIgnore]
        private FenChessboard chessBoard;

        public CustomVariation()
        {

        }

        public CustomVariation(Course course)
        {
            Course = course;
        }

        public CustomVariation(FenChessboard chessBoard, VerticalStackLayout verticalStackLayout, Course course)
        {
            this.chessBoard = chessBoard;
            this.verticalStackLayout = verticalStackLayout;
            Course = course;
        }

        public void Initialize(FenChessboard chessBoard, VerticalStackLayout verticalStackLayout)
        {
            this.chessBoard = chessBoard;
            this.verticalStackLayout = verticalStackLayout;
        }

        public void SubscribeToEvents(params object[] subscribers)
        {
            (subscribers.First() as MoveHistory).AddedMove += MoveHistory_AddedMove;
            (subscribers.First() as MoveHistory).RequestingPreviousMove += MoveHistory_RequestingPreviousMove;
        }

        public string GetStartingFen()
        {
            return moves.First().fen;
        }

        public string GetLastFen()
        {
            MoveHistory.Move lastMove = moves.Last();
            return lastMove.fenSettings.GetLichessFen(lastMove.fen);
        }

        public FenSettings GetLastFenSettings()
        {
            return moves.Last().fenSettings;
        }

        private void MoveHistory_RequestingPreviousMove(MoveHistory.Move currentMove, MoveHistory.Move previousMove)
        {
            moves.Remove(currentMove);
            ReloadListButtons();
        }

        private void MoveHistory_AddedMove(MoveHistory.Move historyMove)
        {
            moves.Add(historyMove);
            if (PreviewFen == "")
                PreviewFen = moves.First().fen;
            ReloadListButtons();
        }

        public void ReloadListButtons()
        {
            verticalStackLayout.Clear();
            for (int i = 0; i < moves.Count; i++)
            {
                string fen = moves[i].fen.Split(' ')[0] + moves[i].fenSettings.GetAppliedSettings(" ");

                var listButton = new CustomVariationMoveButton(chessBoard, fen, moves[i].moveNotation, i);
                listButton.CustomVariationButtonClicked += ListButton_CustomVariationButtonClicked;
                verticalStackLayout.Add(listButton.button);
            }

            //verticalStackLayout.HeightRequest = ListButton.BUTTON_HEIGHT * moves.Count;
        }

        private void ListButton_CustomVariationButtonClicked(string fen)
        {
            chessBoard.LoadChessBoardFromFen(fen);
            PreviewFen = fen;
        }

        public string GetKey(CustomVariation customVariation)
        {
            return customVariation.Course.Name + ID;
        }
    }
}
