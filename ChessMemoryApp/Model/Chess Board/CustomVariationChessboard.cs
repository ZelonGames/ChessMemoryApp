using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class CustomVariationChessboard : CourseChessboard
    {
        public delegate void ClickedEventHandler(CustomVariationChessboard customVariationChessBoard);
        public new event ClickedEventHandler Clicked;
        public event ClickedEventHandler DeleteClicked;
        public event ClickedEventHandler EditClicked;

        public readonly CustomVariation customVariation;
        private readonly UICustomVariationChessBoard uiCustomVariationChessBoard;
        private readonly UISortingButtonsChessBoard uiSortingButtonsChessBoard;

        private bool hasMoved = false;

        public CustomVariationChessboard(ChessboardGenerator chessBoard, CustomVariation customVariation, AbsoluteLayout chessBoardListLayout, Size size) : 
            base(chessBoard, customVariation.Course, chessBoardListLayout, size)
        {
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            this.customVariation = customVariation;
            uiCustomVariationChessBoard = new UICustomVariationChessBoard(chessBoardListLayout, customVariation);
            uiTitleChessBoard.DisableTitle();
            /*
            uiSortingButtonsChessBoard = new UISortingButtonsChessBoard(chessBoardListLayout);
            uiSortingButtonsChessBoard.AddGestureRecognizers(tapGestureRecognizer);
            uiSortingButtonsChessBoard.PointerExited += ResetPosition;*/
            
            uiCustomVariationChessBoard.EditClicked += UiCustomVariationChessBoard_EditClicked;
            uiCustomVariationChessBoard.DeleteClicked += UiCustomVariationChessBoard_DeleteClicked;
            uiCustomVariationChessBoard.DeleteButtonEntered += UiCustomVariationChessBoard_DeleteButtonEntered;
            uiCustomVariationChessBoard.DeleteButtonExited += UiCustomVariationChessBoard_DeleteButtonExited;
            uiCustomVariationChessBoard.HideUI();
        }

        private void ResetPosition()
        {
            if (hasMoved)
            {
                offset.X -= 20;
                UpdateSquaresBounds();
                uiSortingButtonsChessBoard.HideUI();
                hasMoved = false;
            }
        }

        public CustomVariationChessboard(ChessboardGenerator chessBoard, AbsoluteLayout chessBoardListLayout, Size size) :
            base(chessBoard, null, chessBoardListLayout, size)
        {
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
        }

        public override void OnBoardEntered(object sender, EventArgs e)
        {
            base.OnBoardEntered(sender, e);
            if (!hasMoved && uiSortingButtonsChessBoard != null)
            {
                offset.X += 20;
                hasMoved = true;
                UpdateSquaresBounds();
            }
            uiCustomVariationChessBoard?.ShowUI();
            uiSortingButtonsChessBoard?.ShowUI();
        }

        public override void OnBoardExited(object sender, EventArgs e)
        {
            base.OnBoardExited(sender, e);
            uiCustomVariationChessBoard?.HideUI();
        }

        public override void UpdateSquaresBounds()
        {
            base.UpdateSquaresBounds();
            uiCustomVariationChessBoard?.UpdateUIPosition(offset);
            uiSortingButtonsChessBoard?.UpdateUIPosition(BoardSize, offset);
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            Clicked?.Invoke(this);
        }

        private void UiCustomVariationChessBoard_DeleteButtonExited(Button button, CustomVariation customVariation)
        {
            base.OnBoardExited(null, null);
            uiCustomVariationChessBoard?.HideUI();
        }

        private void UiCustomVariationChessBoard_DeleteButtonEntered(Button button, CustomVariation customVariation)
        {
            base.OnBoardEntered(null, null);
            uiCustomVariationChessBoard?.ShowUI();
        }

        private void UiCustomVariationChessBoard_DeleteClicked(CustomVariation customVariation)
        {
            DeleteClicked?.Invoke(this);
        }

        private void UiCustomVariationChessBoard_EditClicked(CustomVariation customVariation)
        {
            EditClicked?.Invoke(this);
        }
    }
}
