using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Components
{
    public class CustomVariationMoveButton : ListButton
    {
        public delegate void CustomVariationButtonClickedEventHandler(string fen);
        public event CustomVariationButtonClickedEventHandler CustomVariationButtonClicked;

        private readonly CustomVariation customVariation;
        private readonly FenChessboard chessBoard;
        public readonly string fen;

        public CustomVariationMoveButton(CustomVariation customVariation, FenChessboard chessBoard, string fen, string moveNotation, int buttonIndex) :
            base(moveNotation, buttonIndex)
        {
            this.customVariation = customVariation;
            this.chessBoard = chessBoard;
            this.fen = fen;
            PointerGestureRecognizer.PointerEntered += PointerGestureRecognizer_PointerEntered;
            PointerGestureRecognizer.PointerExited += PointerGestureRecognizer_PointerExited;

            button.Clicked += Button_Clicked;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            CustomVariationButtonClicked?.Invoke(fen);
        }

        private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
        {
            if (!FenHelper.IsValidFen(customVariation.PreviewFen))
                chessBoard.ClearPieces();
            else
                chessBoard.LoadChessBoardFromFen(customVariation.PreviewFen);
        }

        private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
        {
            chessBoard.LoadChessBoardFromFen(fen);
        }
    }
}
