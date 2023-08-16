using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
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

        private readonly FenChessboard chessBoard;
        private readonly string fen;

        public string FEN => fen;

        public CustomVariationMoveButton(FenChessboard chessBoard, string fen, string moveNotation, int buttonIndex) :
            base(moveNotation, buttonIndex)
        {
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
            if (!FenHelper.IsValidFen(chessBoard.currentFen))
                chessBoard.ClearPieces();
            else
                chessBoard.LoadTemporaryFen(chessBoard.currentFen);
        }

        private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
        {
            chessBoard.LoadTemporaryFen(fen);
        }
    }
}
