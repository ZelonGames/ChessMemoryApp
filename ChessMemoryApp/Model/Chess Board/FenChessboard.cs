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
        // TODO: fix colortoplay
        public FenChessboard(AbsoluteLayout chessBoardListLayout, Size size, Piece.ColorType colorToPlay) : 
            base(chessBoardListLayout, size, colorToPlay == Piece.ColorType.Black)
        {
            this.boardColorOrientation = colorToPlay;
            chessBoardListLayout.WidthRequest = size.Width;
            chessBoardListLayout.HeightRequest = size.Height + 10;
        }
    }
}
