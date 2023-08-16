using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Helpers.Main_Page
{
    public class UIEventHelper
    {
        public UIEventHelper() { }

        #region ContentView Events

        public static void ContentViewClickSubscribe(ContentView contentView, EventHandler<TappedEventArgs> handler)
        {
            TapGestureRecognizer tapGestureRecognizer = new();
            tapGestureRecognizer.Tapped += handler;
            contentView.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public static void ContentViewClickUnSubscribe(ContentView contentView, EventHandler<TappedEventArgs> handler)
        {/*
            TapGestureRecognizer tapGestureRecognizer = new();
            tapGestureRecognizer.Tapped -= handler;
                contentView.GestureRecognizers.Add(tapGestureRecognizer);*/
        }

        #endregion

        #region Image Events

        public static void ImageClickSubscribe(Image image, EventHandler<TappedEventArgs> handler)
        {
            TapGestureRecognizer tapGestureRecognizer = new();
            tapGestureRecognizer.Tapped += handler;
            image.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public static void ImageClickUnSubscribe(Image image, EventHandler<TappedEventArgs> handler)
        {/*
            TapGestureRecognizer tapGestureRecognizer = new();
            tapGestureRecognizer.Tapped -= handler;
            image.GestureRecognizers.Where()*/
        }

        #endregion

        #region Button Events

        public static void ButtonMouseEnterSubscribe(Button button, EventHandler<PointerEventArgs> handler)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new();
            pointerGestureRecognizer.PointerEntered += handler;
            button.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        public static void ButtonMouseLeaveSubscribe(Button button, EventHandler<PointerEventArgs> handler)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new();
            pointerGestureRecognizer.PointerExited += handler;
            button.GestureRecognizers.Add(pointerGestureRecognizer);
        }
        public static void ButtonMouseLeaveUnSubscribe(Button button, EventHandler<PointerEventArgs> handler)
        {
            PointerGestureRecognizer pointerGestureRecognizer = new();
            pointerGestureRecognizer.PointerExited -= handler;
            button.GestureRecognizers.Add(pointerGestureRecognizer);
        }

        public static void ButtonMouseClickSubscribe(Button button, EventHandler handler)
        {
            button.Clicked += handler;
        }

        #endregion
    }
}
