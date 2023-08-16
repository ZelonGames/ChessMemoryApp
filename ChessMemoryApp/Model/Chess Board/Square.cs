using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model
{
    public class Square
    {
        public ContentView contentView;
        public Piece.Coordinates<int> coordinate;
        public readonly Color initialColor;
        private MoveNotationGenerator moveNotationHelper;

        public static Square HighlightedSquare { get; private set; }

        public Square(ContentView contentView, Piece.Coordinates<int> coordinate, bool isClickable = true)
        {
            var pointer = new PointerGestureRecognizer();

            this.contentView = contentView;
            this.coordinate = coordinate;
            initialColor = contentView.BackgroundColor;

            if (isClickable)
            {
                pointer.PointerEntered += Pointer_PointerEntered;
                pointer.PointerExited += Pointer_PointerExited;
                contentView.GestureRecognizers.Add(pointer);
                UIEventHelper.ContentViewClickSubscribe(this.contentView, OnPictureBoxClicked);
            }
        }

        public void HighlightSquare()
        {
            HighlightedSquare = this;
            contentView.BackgroundColor = GetDarkerColor();
        }

        public void LowlightSquare()
        {
            contentView.BackgroundColor = initialColor;
            HighlightedSquare = null;
        }

        private void Pointer_PointerExited(object sender, PointerEventArgs e)
        {
            if (HighlightedSquare != this)
                contentView.BackgroundColor = initialColor;
        }

        private void Pointer_PointerEntered(object sender, PointerEventArgs e)
        {
            contentView.BackgroundColor = GetDarkerColor();
        }

        public void SetMoveNotationGenerator(MoveNotationGenerator moveNotationHelper)
        {
            this.moveNotationHelper = moveNotationHelper;
        }

        public void RemovePiece()
        {
            contentView.Content = null;
        }

        public void OnPictureBoxClicked(object sender, EventArgs e)
        {
            if (moveNotationHelper == null)
                return;

            if (!moveNotationHelper.IsFirstClick)
            {
                moveNotationHelper.SetSecondClick(coordinate);
                HighlightedSquare?.LowlightSquare();
            }
        }

        private Color GetDarkerColor()
        {
            float darkness = 0.2f;

            return Color.FromRgba(
                initialColor.Red - darkness,
                initialColor.Green - darkness,
                initialColor.Blue - darkness,
                initialColor.Alpha);
        }
    }
}
