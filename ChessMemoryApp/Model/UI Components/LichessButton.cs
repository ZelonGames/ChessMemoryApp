using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using Color = Microsoft.Maui.Graphics.Color;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.Chess_Board;

namespace ChessMemoryApp.Model.UI_Components
{
    /// <summary>
    /// Requests the next fen you would get after making the next move
    /// </summary>
    public class LichessButton
    {
        public readonly ChessboardGenerator chessBoard;
        public readonly ExplorerMove move;
        public readonly Button button;

        public delegate void RequestNewFenEventHandler(string fen, ExplorerMove move);
        public static event RequestNewFenEventHandler RequestedNewFen;

        private bool isClickingButton = false;
        private readonly string initialFen;
        private string previewFen;

        public string Text => button.Text;

        public LichessButton(ChessboardGenerator chessBoard, Button button, ExplorerMove move)
        {
            initialFen = chessBoard.currentFen;
            this.chessBoard = chessBoard;
            this.move = move;
            this.button = button;

            button.Clicked += RequestNewFen;

            var pointer = new PointerGestureRecognizer();
            pointer.PointerEntered += OnPointerEntered;
            pointer.PointerExited += OnPointerExited;
            button.GestureRecognizers.Add(pointer);
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            if (!isClickingButton)
                chessBoard.LoadPieces(previewFen, initialFen);
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            if (!isClickingButton)
            {
                if (previewFen == null)
                    previewFen = FenHelper.MakeMoveWithCoordinates(initialFen, move.MoveNotationCoordinates);

                chessBoard.LoadPieces(initialFen, previewFen);
            }
        }

        public void RequestNewFen(object sender, EventArgs e)
        {
            chessBoard.LoadChessBoardFromFen(initialFen);
            isClickingButton = true;
            string newFen = FenHelper.MakeMoveWithCoordinates(initialFen, move.MoveNotationCoordinates);
            RequestedNewFen?.Invoke(newFen, move);
        }
    }
}
