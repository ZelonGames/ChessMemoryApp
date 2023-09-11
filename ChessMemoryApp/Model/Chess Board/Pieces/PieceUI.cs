using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.Model.UI_Helpers.Main_Page;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board.Pieces
{
    public class PieceUI
    {
        public static readonly Dictionary<char, string> pieceFileNames = new()
        {
            { 'p', "pawn_black" },
            { 'P', "pawn_white" },
            { 'r', "rook_black" },
            { 'R', "rook_white" },
            { 'n', "knight_black" },
            { 'N', "knight_white" },
            { 'b', "bishop_black" },
            { 'B', "bishop_white" },
            { 'q', "queen_black" },
            { 'Q', "queen_white" },
            { 'k', "king_black" },
            { 'K', "king_white" },
        };

        public readonly string coordinates;
        public readonly Image image;
        protected readonly UIChessBoard chessBoard;

        public PieceUI(UIChessBoard chessBoard, Piece pieceData)
        {
            this.chessBoard = chessBoard;
            this.coordinates = pieceData.coordinates;

            pieceFileNames.TryGetValue(pieceData.pieceType, out string fileName);
            image = new Image()
            {
                Source = ImageSource.FromFile(fileName + ".png"),
            };

            TapGestureRecognizer tapGestureRecognizer = new();
            tapGestureRecognizer.Tapped += OnPieceClicked;
            image.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public static Piece.ColorType GetColorOfPieceFromFen(string fen, string coordinates)
        {
            char? piece = FenHelper.GetPieceOnSquare(fen, coordinates);
            if (!piece.HasValue)
                return Piece.ColorType.Empty;

            return char.IsUpper(piece.Value) ? Piece.ColorType.White : Piece.ColorType.Black;
        }

        public void UnsubscribeEvents()
        {
            UIEventHelper.ImageClickUnSubscribe(image, OnPieceClicked);
            image.GestureRecognizers.Clear();
        }

        public void OnPieceClicked(object sender, EventArgs e)
        {
            if (chessBoard.moveNotationHelper == null)
                return;

            bool clickedOnPiece = chessBoard.squares[coordinates] != null;

            if (clickedOnPiece && chessBoard.moveNotationHelper.IsFirstClick)
            {
                chessBoard.moveNotationHelper.SetFirstClick(BoardHelper.GetNumberCoordinates(coordinates));
                chessBoard.squares[coordinates].HighlightSquare();
            }
            else
            {
                chessBoard.moveNotationHelper.SetSecondClick(BoardHelper.GetNumberCoordinates(coordinates));
                Square.HighlightedSquare?.LowlightSquare();
            }
        }
    }
}
