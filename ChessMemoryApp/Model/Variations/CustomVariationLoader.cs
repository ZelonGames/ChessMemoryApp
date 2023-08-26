using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.UI_Helpers;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using ChessMemoryApp.Model.UI_Components;

namespace ChessMemoryApp.Model.Variations
{
    /// <summary>
    /// Makes sure to load lichess moves using Lichess API at the right moments
    /// </summary>
    public class VariationLoader
    {
        public event Action FinishedLoadingLichess;
        public event Action LoadingLichess;

        public readonly List<LichessButton> lichessButtons = new();
        public readonly ChessboardGenerator chessBoard;
        public readonly VerticalStackLayout verticalStackLayout;

        public bool IsLoadingLichess { get; private set; }

        public VariationLoader(VerticalStackLayout verticalStackLayout, ChessboardGenerator chessBoard)
        {
            this.verticalStackLayout = verticalStackLayout;
            this.chessBoard = chessBoard;
        }

        public void SubscribeToEvents(LichessMoveExplorer lichessMoveExplorer, PieceMover pieceMover, MoveHistory moveHistory)
        {
            lichessMoveExplorer.RecevedLichessMoves += LoadLichessVariations;
            pieceMover.MadeChessableMove += OnMadeChessableMove;
            moveHistory.RequestedPreviousMove += OnRequestedPreviousMove;

            LichessButton.RequestedNewFen += (fen, move) => { ClearLichessMoves(); };
        }

        private async void OnRequestedPreviousMove(MoveHistory.Move historyMove)
        {
            if (historyMove.moveSource.Equals(MoveHistory.MoveSource.Chessable))
            {
                AddLoadingText();
                IsLoadingLichess = true;
                LoadingLichess?.Invoke();

                var task = Task.Run(async () =>
                {
                    OpeningExplorer openingExplorer = await LichessRequestHelper.GetOpeningMoves(
                        historyMove.fenSettings, historyMove.fen);

                    return openingExplorer;
                });

                OpeningExplorer openingExplorer = await task;
                IsLoadingLichess = false;
                LoadLichessVariations(historyMove.fen, openingExplorer);
                FinishedLoadingLichess?.Invoke();
            }
            else
                ClearLichessMoves();
        }

        private async void OnMadeChessableMove(Move move)
        {
            AddLoadingText();
            IsLoadingLichess = true;
            LoadingLichess?.Invoke();

            var task = Task.Run(async () =>
            {
                OpeningExplorer openingExplorer = await LichessRequestHelper.GetOpeningMoves(
                    chessBoard.fenSettings, chessBoard.currentFen);

                return openingExplorer;
            });

            OpeningExplorer openingExplorer = await task;
            LoadLichessVariations(move.Fen, openingExplorer);
            IsLoadingLichess = false;
            FinishedLoadingLichess?.Invoke();
        }

        public void LoadLichessVariations(string currentFen, OpeningExplorer openingExplorer)
        {
            ClearLichessMoves();
            if (openingExplorer == null)
                return;

            foreach (var move in openingExplorer.Moves)
            {
                var button = new ListButton(move.MoveNotation + " Wins: " + move.GetWinsInPercent(chessBoard.playAsBlack), lichessButtons.Count);
                var lichessButton = new LichessButton(chessBoard, button.button, move);
                lichessButtons.Add(lichessButton);
                verticalStackLayout.Add(lichessButton.button);
            }
        }

        private void AddLoadingText()
        {
            var lblLoading = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Loading Lichess Variations...",
                TextColor = Color.FromArgb("CCCCCC"),
                FontSize = 18
            };
            verticalStackLayout.Add(lblLoading);
        }

        private void ClearLichessMoves()
        {
            verticalStackLayout.Clear();
            lichessButtons.Clear();
        }
    }
}
