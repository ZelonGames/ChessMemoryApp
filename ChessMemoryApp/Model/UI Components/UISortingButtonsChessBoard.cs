using ChessMemoryApp.Model.Chess_Board.Pieces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class UISortingButtonsChessBoard
    {
        public event Action PointerExited;

        private readonly ContentView leftButton = new();
        private readonly ContentView rightButton = new();

        public UISortingButtonsChessBoard(AbsoluteLayout chessBoardLayout)
        {
            leftButton.WidthRequest = 20;
            leftButton.HeightRequest = chessBoardLayout.HeightRequest;
            leftButton.HorizontalOptions = LayoutOptions.Center;
            leftButton.VerticalOptions = LayoutOptions.Center;
            chessBoardLayout.Add(leftButton);
        }

        public void AddGestureRecognizers(TapGestureRecognizer tapGestureRecognizer)
        {
            leftButton.GestureRecognizers.Add(tapGestureRecognizer);

            var clickGestureRecognizer = new ClickGestureRecognizer();
            clickGestureRecognizer.Clicked += Clicked;
            leftButton.GestureRecognizers.Add(clickGestureRecognizer);

            var pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerExited += Exited;
            leftButton.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        private void Exited(object sender, PointerEventArgs e)
        {
            PointerExited?.Invoke();
        }

        private void Clicked(object sender, EventArgs e)
        {
        }

        public void ClearGestureRecognizers()
        {
            leftButton.GestureRecognizers.Clear();
        }

        public void ShowUI()
        {
            leftButton.BackgroundColor = Color.FromRgba(0, 0, 0, 255);
        }

        public void HideUI()
        {
            leftButton.BackgroundColor = Color.FromRgba(0, 0, 0, 0);
        }

        public void UpdateUIPosition(Size boardSize, Piece.Coordinates<double> offset)
        {
            leftButton.HeightRequest = boardSize.Height;
            leftButton.TranslationX = offset.X - leftButton.WidthRequest;
            leftButton.TranslationY = offset.Y;
            leftButton.ZIndex = 1;
        }
    }
}
