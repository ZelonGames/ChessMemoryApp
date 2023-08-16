using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public static class PieceHelper
    {
        public static string GetPieceName(char pieceChar)
        {
            switch (char.ToLower(pieceChar))
            {
                case 'r':
                    return "Rook";
                case 'n':
                    return "Knight";
                case 'b':
                    return "Bishop";
                case 'q':
                    return "Queen";
                case 'k':
                    return "King";
                case 'p':
                    return "Pawn";
                default:
                    break;
            }

            return "";
        }

        public static Piece.ColorType GetOppositeColor(Piece.ColorType color)
        {
            return color == Piece.ColorType.White ? Piece.ColorType.Black : Piece.ColorType.White;
        }
    }
}
