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
    public class UICourseChessBoard
    {
        private readonly Label labelTextName;
        private readonly ContentView clickView = new();

        public UICourseChessBoard(Size size, AbsoluteLayout chessBoardLayout, string text)
        {
            labelTextName = new()
            {
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 60
            };
            if (chessBoardLayout != null)
            {
                labelTextName.WidthRequest = labelTextName.MaximumWidthRequest = size.Width;
                labelTextName.HorizontalTextAlignment = TextAlignment.Center;
                labelTextName.VerticalTextAlignment = TextAlignment.Center;
                labelTextName.Text = text;
                labelTextName.BackgroundColor = Color.FromRgba(0, 0, 0, 0);
                clickView.Content = labelTextName;
                clickView.HorizontalOptions = LayoutOptions.Center;
                clickView.VerticalOptions = LayoutOptions.Center;
                chessBoardLayout.Add(clickView);
                ShowUI();
            }
        }

        public void AddGestureRecognizers(
            TapGestureRecognizer tapGestureRecognizer,
            EventHandler<PointerEventArgs> OnBoardEntered,
            EventHandler<PointerEventArgs> OnBoardExited)
        {
            clickView.GestureRecognizers.Add(tapGestureRecognizer);

            var pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += OnBoardEntered;
            pointerGestureRecognizer.PointerExited += OnBoardExited;
            clickView.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        public void ClearGestureRecognizers()
        {
            clickView.GestureRecognizers.Clear();
        }

        public void ShowUI()
        {
            clickView.BackgroundColor = Color.FromRgba(0, 0, 0, 60);
            if (labelTextName.Text.Length > 0)
                labelTextName.BackgroundColor = Color.FromRgba(255, 255, 255, 200);
            labelTextName.TextColor = Color.FromRgba("000");
        }

        public void HideUI()
        {
            clickView.BackgroundColor = Color.FromRgba(0, 0, 0, 0);
            if (labelTextName.Text.Length > 0)
                labelTextName.BackgroundColor = Color.FromRgba(255, 255, 255, 0);
            labelTextName.TextColor = Color.FromRgba(255, 255, 255, 0);
        }

        public void UpdateUIPosition(Size boardSize, Piece.Coordinates<double> offset)
        {
            clickView.BackgroundColor = Color.FromRgba(0, 0, 0, 60);
            clickView.WidthRequest = boardSize.Width;
            clickView.HeightRequest = boardSize.Height;
            labelTextName.WidthRequest = labelTextName.MaximumWidthRequest = boardSize.Width;
            clickView.TranslationX = offset.X;
            clickView.TranslationY = offset.Y;
            clickView.ZIndex = 1;
        }
    }
}
