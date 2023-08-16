using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class PieceMoveHandler
    {
        private static BoxView currentPieceToMove;
        private readonly AbsoluteLayout chessBoard;

        private Point? currentMousePosition = null;
        private Point? currentPiecePosition = null;
        private readonly PanGestureRecognizer panGesture = new();

        public PieceMoveHandler(AbsoluteLayout chessBoard)
        {
            this.chessBoard = chessBoard;
            panGesture.PanUpdated += OnMouseDragging;
        }

        public void OnPointerEntered(object sender, PointerEventArgs e)
        {
            // Handle the pointer entered event
            if (sender is BoxView)
            {
                currentPieceToMove = sender as BoxView;
                currentPieceToMove.GestureRecognizers.Add(panGesture);
            }
        }

        public void OnPointerExited(object sender, PointerEventArgs e)
        {
            panGesture.PanUpdated -= OnMouseDragging;
            currentPieceToMove.GestureRecognizers.Remove(panGesture);
            currentPieceToMove = null;
        }

        public void OnPointerMoved(object sender, PointerEventArgs e)
        {
            currentMousePosition = e.GetPosition(currentPieceToMove);
        }

        Point startDragPoint = new(0, 0);
        public void OnMouseDragging(object sender, PanUpdatedEventArgs e)
        {

            // Check if the PanGesture was triggered by a mouse event
            if (e.StatusType == GestureStatus.Started)
            {
                // Store the starting point of the drag
                startDragPoint = new Point(e.TotalX, e.TotalY);
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                var distanceMouseMoved = new Point(e.TotalX - startDragPoint.X, e.TotalY - startDragPoint.Y);

                // Update the translation of the box
                currentPieceToMove.TranslationX += distanceMouseMoved.X;
                currentPieceToMove.TranslationY += distanceMouseMoved.Y;

                // Update the starting point of the drag
                startDragPoint = new Point(e.TotalX, e.TotalY);
            }
        }
    }
}
