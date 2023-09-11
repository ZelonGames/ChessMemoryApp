using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class Queen : Piece
    {
        public Queen(ChessboardGenerator chessBoard, char pieceType) : base(chessBoard, pieceType)
        {

        }

        public override HashSet<string> GetAvailableMoves()
        {
            var availableMoves = new HashSet<string>();

            var rook = new Rook(chessBoard, color == ColorType.White ? 'R' : 'r');
            rook.coordinates = coordinates;
            var bishop = new Bishop(chessBoard, color == ColorType.White ? 'B' : 'b');
            bishop.coordinates = coordinates;

            availableMoves.UnionWith(rook.GetAvailableMoves());
            availableMoves.UnionWith(bishop.GetAvailableMoves());

            return availableMoves;
        }
    }
}
