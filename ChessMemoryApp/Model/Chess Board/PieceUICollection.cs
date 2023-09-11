using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.UI_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class PieceUICollection
    {
        private readonly Dictionary<string, PieceUI> piecesUI = new();
        private readonly UIChessBoard chessBoardUI;

        public PieceUICollection(UIChessBoard chessBoardUI)
        {
            this.chessBoardUI = chessBoardUI;
        }

        public PieceUI GetPiece(string coordinates)
        {
            piecesUI.TryGetValue(coordinates, out PieceUI piece);
            return piece;
        }

        public bool PieceExists(string coordinates)
        {
            return piecesUI.ContainsKey(coordinates);
        }

        public void AddPieceToSquare(Piece pieceData, Square square)
        {
            var pieceUI = new PieceUI(chessBoardUI, pieceData);
            square.contentView.Content = pieceUI.image;
            piecesUI.Add(pieceData.coordinates, pieceUI);
        }

        public void AddPiece(string coordinates, char pieceType)
        {
            chessBoardUI.chessBoardData.TryAddPieceToSquare(pieceType, coordinates);
            Piece pieceData = chessBoardUI.chessBoardData.GetPiece(coordinates);

            var pieceUI = new PieceUI(chessBoardUI, pieceData);
            Square square = chessBoardUI.squares[pieceData.coordinates];
            square.contentView.Content = pieceUI.image;
            piecesUI.Add(pieceData.coordinates, pieceUI);
        }

        public void ClearPieces()
        {
            foreach (var piece in piecesUI.Values)
                RemovePiece(piecesUI[piece.coordinates]);

            piecesUI.Clear();
        }

        public void RemovePiece(PieceUI piece)
        {
            if (piece == null)
                return;

            piece.UnsubscribeEvents();
            chessBoardUI.squares[piece.coordinates].ClearContent();
            piecesUI.Remove(piece.coordinates);
        }
    }
}
