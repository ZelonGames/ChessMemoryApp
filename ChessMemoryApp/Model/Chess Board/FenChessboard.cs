using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class FenChessboard : UIChessBoard
    {
        public FenChessboard(ChessboardGenerator chessBoard, AbsoluteLayout chessBoardListLayout, Size size) : 
            base(chessBoard, chessBoardListLayout, size)
        {
            chessBoardListLayout.WidthRequest = size.Width;
            chessBoardListLayout.HeightRequest = size.Height + 10;
        }
    }
}
