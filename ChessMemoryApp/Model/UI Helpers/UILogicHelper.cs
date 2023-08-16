using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Variations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Helpers
{
    public class UILogicHelper
    {
        public UILogicHelper() { }

        public static void RemovePieceFromSquare(ChessboardGenerator chessBoard, Piece piece)
        {
            string letterCoordinates = BoardHelper.GetLetterCoordinates(piece.currentCoordinate);
            chessBoard.squares[letterCoordinates].RemovePiece();
        }

        public static void RemoveButtonFromVariationLoader(VariationLoader variationLoader, Button button)
        {

        }

        public static void AddButtonToVariationLoader(VariationLoader variationLoader, Button button)
        {

        }

        public static Size GetVariationLoaderSize(VariationLoader variationLoader)
        {
            return new Size();
        }
    }
}
