using ChessMemoryApp.Model.Memory_Techniques;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class UIMemoryPalaceWordButton : ListButton
    {
        public delegate void ButtonInteractionHandler(UIMemoryPalaceWordButton interactedButton);
        public event ButtonInteractionHandler Clicked;
        public event ButtonInteractionHandler PointerEntered;
        public event ButtonInteractionHandler PointerExited;

        public UIMemoryPalaceWordButton(string coordinates) : base(coordinates, 1)
        {
            button.MinimumWidthRequest = 50;
            button.MaximumWidthRequest = 200;
            button.HeightRequest = 50;
            button.Padding = 2;

            var pointer = new PointerGestureRecognizer();
            pointer.PointerEntered += OnPointerEntered;
            pointer.PointerExited += OnPointerExited;
            var click = new ClickGestureRecognizer();
            click.Clicked += OnClicked;
            button.GestureRecognizers.Add(pointer);

            button.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            double a = (sender as Button).Width;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this);
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            PointerEntered?.Invoke(this);
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            PointerExited?.Invoke(this);
        }
    }
}
