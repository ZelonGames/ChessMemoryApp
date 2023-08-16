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
        public FenChessboard(AbsoluteLayout chessBoardLayout, Size size, bool playAsBlack) : 
            base(chessBoardLayout, size)
        {
            this.playAsBlack = playAsBlack;
        }
    }
}
