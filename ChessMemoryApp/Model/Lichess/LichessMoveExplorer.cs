using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.Lichess.Lichess_API;

namespace ChessMemoryApp.Model.Lichess
{
    public class LichessMoveExplorer : IEventController
    {
        public delegate void ReceivedLichessMovesEventHandler(string currentFen, OpeningExplorer openingExplorer);
        public event ReceivedLichessMovesEventHandler RecevedLichessMoves;

        private ChessboardGenerator chessBoard;

        public LichessMoveExplorer(ChessboardGenerator chessBoard)
        {
            this.chessBoard = chessBoard;
        }

        public void SubscribeToEvents(params object[] subscribers)
        {
            PieceMover pieceMover = subscribers.First() as PieceMover;
            pieceMover.MovedPiece += GetLichessMoves;
        }

        public async void GetLichessMoves(string fen)
        {            
            OpeningExplorer openingExplorer = await LichessRequestHelper.GetOpeningMoves(
                chessBoard.fenSettings, fen);

            RecevedLichessMoves?.Invoke(fen, openingExplorer);
        }
    }
}
