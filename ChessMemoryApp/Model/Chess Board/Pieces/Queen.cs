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
        public Queen(ChessboardGenerator chessBoard, ColorType color) : base(chessBoard, color, 'q')
        {

        }

        public static HashSet<string> GetAvailableMoves(string pieceLetterCoordinates, string fen)
        {
            var availableMoves = new HashSet<string>();

            availableMoves.UnionWith(Bishop.GetAvailableMoves(pieceLetterCoordinates, fen));
            availableMoves.UnionWith(Rook.GetAvailableMoves(pieceLetterCoordinates, fen));
            

            return availableMoves;
        }
    }
}
