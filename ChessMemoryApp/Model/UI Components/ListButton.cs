using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class ListButton
    {
        public const double BUTTON_HEIGHT = 50;
        public Button button;

        protected PointerGestureRecognizer PointerGestureRecognizer { get; private set; }

        private readonly int buttonIndex;

        private readonly Color darkColor = Color.FromArgb("#424242");
        private readonly Color lightColor = Color.FromArgb("#505050");
        private readonly Color highlightedColor = Color.FromArgb("#D3D3D3");

        public ListButton(string moveNotation, int buttonIndex)
        {
            this.buttonIndex = buttonIndex;
            button = new Button();
            button.Text = moveNotation;
            button.HeightRequest = BUTTON_HEIGHT;
            button.CornerRadius = 0;
            button.BorderWidth = 0;

            if (buttonIndex % 2 == 0)
                button.BackgroundColor = darkColor;
            else
                button.BackgroundColor = lightColor;

            PointerGestureRecognizer = new PointerGestureRecognizer();
            PointerGestureRecognizer.PointerEntered += HighlightButton;
            PointerGestureRecognizer.PointerExited += UnHighlightButton;
            button.GestureRecognizers.Add(PointerGestureRecognizer);
        }

        public void HighlightButton(object sender, EventArgs args)
        {
            var button = sender as Button;
            button.BackgroundColor = highlightedColor;
            button.TextColor = Color.FromArgb("000");
        }

        public void UnHighlightButton(object sender, EventArgs args)
        {
            var button = sender as Button;
            if (buttonIndex % 2 == 0)
                button.BackgroundColor = darkColor;
            else
                button.BackgroundColor = lightColor;
            button.TextColor = Color.FromArgb("fff");
        }

    }
}
