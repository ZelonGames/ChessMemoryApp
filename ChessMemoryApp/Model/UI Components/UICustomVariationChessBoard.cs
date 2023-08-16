using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class UICustomVariationChessBoard
    {
        public delegate void ButtonClickedEventHandler(CustomVariation customVariation);
        public event ButtonClickedEventHandler DeleteClicked;
        public event ButtonClickedEventHandler EditClicked;

        public delegate void DeletePointerEventHandler(Button button, CustomVariation customVariation);
        public event DeletePointerEventHandler DeleteButtonEntered;
        public event DeletePointerEventHandler DeleteButtonExited;

        public readonly Button deleteButton;
        public readonly Button editButton;
        private readonly CustomVariation customVariation;

        public UICustomVariationChessBoard(AbsoluteLayout chessBoardLayout, CustomVariation customVariation)
        {
            this.customVariation = customVariation;
            deleteButton = new Button()
            {
                BackgroundColor = Color.FromArgb("FF5733"),
                TextColor = Color.FromArgb("fff"),
                Text = "X",
                FontAttributes = FontAttributes.Bold,
                BorderWidth = 0,
                WidthRequest = 25,
                HeightRequest = 25,
                TranslationX = 0,
                TranslationY = 0,
            };

            editButton = new Button()
            {
                BackgroundColor = Color.FromArgb("0066CC"),
                TextColor = Color.FromArgb("fff"),
                Text = "//",
                FontAttributes = FontAttributes.Bold,
                BorderWidth = 0,
                WidthRequest = 25,
                HeightRequest = 25,
                TranslationX = 0,
                TranslationY = 0,
            };

            var pointerGesture = new PointerGestureRecognizer();
            pointerGesture.PointerEntered += PointerGesture_PointerEntered;
            pointerGesture.PointerExited += PointerGesture_PointerExited;
            deleteButton.GestureRecognizers.Add(pointerGesture);
            editButton.GestureRecognizers.Add(pointerGesture);

            deleteButton.Clicked += DeleteButton_Clicked;
            editButton.Clicked += EditButton_Clicked;
            chessBoardLayout.Add(deleteButton);
            chessBoardLayout.Add(editButton);
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            DeleteClicked?.Invoke(customVariation);
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            EditClicked?.Invoke(customVariation);
        }

        private void PointerGesture_PointerExited(object sender, PointerEventArgs e)
        {
            DeleteButtonExited?.Invoke(deleteButton, customVariation);
        }

        private void PointerGesture_PointerEntered(object sender, PointerEventArgs e)
        {
            DeleteButtonEntered?.Invoke(deleteButton, customVariation);
        }

        public void ShowUI()
        {
            deleteButton.Clicked += DeleteButton_Clicked;
            deleteButton.BackgroundColor = Color.FromArgb("f00");
            deleteButton.TextColor = Color.FromRgba("fff");
            deleteButton.ZIndex = 2;

            editButton.Clicked += EditButton_Clicked;
            editButton.BackgroundColor = Color.FromArgb("0066CC");
            editButton.TextColor = Color.FromRgba("fff");
            editButton.ZIndex = 2;
        }

        public void HideUI()
        {
            deleteButton.Clicked -= DeleteButton_Clicked;
            deleteButton.TextColor = Color.FromRgba(0, 0, 0, 0);
            deleteButton.BackgroundColor = Color.FromRgba(0, 0, 0, 0);

            editButton.Clicked -= EditButton_Clicked;
            editButton.TextColor = Color.FromRgba(0, 0, 0, 0);
            editButton.BackgroundColor = Color.FromRgba(0, 0, 0, 0);
        }

        public void UpdateUIPosition(Piece.Coordinates<double> offset)
        {
            deleteButton.TranslationX = offset.X;
            deleteButton.TranslationY = offset.Y;
            deleteButton.ZIndex = 0;

            editButton.TranslationX = offset.X + deleteButton.WidthRequest;
            editButton.TranslationY = offset.Y;
            editButton.ZIndex = 0;
        }
    }
}
