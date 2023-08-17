using ChessMemoryApp.Model.Chess_Board.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class FenChessboard : ChessboardGenerator
    {
        public FenChessboard(AbsoluteLayout chessBoardListLayout, Size size, bool playAsBlack) : 
            base(chessBoardListLayout, size)
        {
            this.playAsBlack = playAsBlack;
            chessBoardListLayout.WidthRequest = size.Width;
            chessBoardListLayout.HeightRequest = size.Height + 10;
        }
    }
}
