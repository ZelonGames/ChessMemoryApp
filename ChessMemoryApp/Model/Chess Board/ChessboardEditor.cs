using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class ChessboardEditor : ChessboardGenerator
    {
        public ChessboardEditor(AbsoluteLayout chessBoardLayout, ColumnDefinition columnChessBoard, bool playAsBlack) : 
            base (chessBoardLayout, columnChessBoard, playAsBlack)
        {

        }
    }
}
