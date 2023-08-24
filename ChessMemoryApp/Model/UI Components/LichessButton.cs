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
                chessBoard.LoadTemporaryFen(chessBoard.currentFen);
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            if (!isClickingButton)
            {
                string newFen = GetNewFenFromMoveNotationCoordinates(move.MoveNotationCoordinates);
                chessBoard.LoadTemporaryFen(newFen);
            }
        }

        public void RequestNewFen(object sender, EventArgs e)
        {
            chessBoard.LoadChessBoardFromFen(initialFen);
            isClickingButton = true;
            string newFen = GetNewFenFromMoveNotationCoordinates(move.MoveNotationCoordinates);
            RequestedNewFen?.Invoke(newFen, move);
        }

        private string GetNewFenFromMoveNotationCoordinates(string moveNotationCoordinates)
        {
            string newFen;
            if (moveNotationCoordinates == "e1h1")
            {
                // White king side castle
                newFen = FenHelper.MakeMoveWithCoordinates(initialFen, "e1g1");
                newFen = FenHelper.MakeMoveWithCoordinates(newFen, "h1f1");
            }
            else if (moveNotationCoordinates == "e1a1")
            {
                // White queen side castle
                newFen = FenHelper.MakeMoveWithCoordinates(initialFen, "e1c1");
                newFen = FenHelper.MakeMoveWithCoordinates(newFen, "a1d1");
            }
            else if (moveNotationCoordinates == "e8h8")
            {
                // Black king side castle
                newFen = FenHelper.MakeMoveWithCoordinates(initialFen, "e8g8");
                newFen = FenHelper.MakeMoveWithCoordinates(newFen, "h8f8");
            }
            else if (moveNotationCoordinates == "e8a8")
            {
                // Black queen side castle
                newFen = FenHelper.MakeMoveWithCoordinates(initialFen, "e8c8");
                newFen = FenHelper.MakeMoveWithCoordinates(newFen, "a8d8");
            }
            else
                newFen = FenHelper.MakeMoveWithCoordinates(initialFen, moveNotationCoordinates);

            return newFen;
        }
    }
}
