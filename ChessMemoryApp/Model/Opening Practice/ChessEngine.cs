using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Game_Analysing;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Opening_Practice
{
    public class ChessEngine
    {
        private Random rnd = new();
        private StockfishEngine stockfishEngine = new();
        public readonly List<Variation> variationsToPractice;
        public readonly ChessboardGenerator chessboardGenerator;
        private string currentFen;

        public ChessEngine(Piece.ColorType boardOrientation, string fenPosition, List<Variation> variationsToPractice)
        {
            chessboardGenerator = new ChessboardGenerator(boardOrientation);
            chessboardGenerator.AddPiecesFromFen(fenPosition);
            this.variationsToPractice = variationsToPractice;
            currentFen = fenPosition;
        }

        public ChessEngine(Piece.ColorType boardOrientation, string fenPosition)
        {
            chessboardGenerator = new ChessboardGenerator(boardOrientation);
            chessboardGenerator.AddPiecesFromFen(fenPosition);
            variationsToPractice = new();
            currentFen = fenPosition;
        }

        public void AddVariation(Chapter chapter, string variationName)
        {
            variationsToPractice.Add(chapter.GetVariation(variationName));
        }

        public void MakeMove(string moveNotationCoordinates)
        {
            chessboardGenerator.MakeMove(moveNotationCoordinates);
            UpdateCurrentFen();
        }

        public void UpdateCurrentFen()
        {
            currentFen = chessboardGenerator.fenSettings.GetLichessFen(chessboardGenerator.GetPositionFen());
        }

        public async Task<string> GetNextEngineMoveFromCurrentFEN()
        {
            List<ExplorerMove> lichessMoves = await GetLichessMovesFromFEN(currentFen);
            string positionFen = currentFen.Split(' ')[0];

            if (lichessMoves.Count == 0)
                return GetStockFishMove(currentFen);

            List<Move> chessableMoves = GetChessableMovesFromFEN(positionFen);

            if (chessableMoves.Count == 0)
                return lichessMoves[rnd.Next(0, Math.Min(lichessMoves.Count, 5))].MoveNotationCoordinates;

            Move randomizedMove = chessableMoves[rnd.Next(0, chessableMoves.Count)];
            string fenColorToPlay = chessboardGenerator.fenSettings.GetColorToPlaySetting();
            Piece.ColorType colorToPlay = fenColorToPlay == "w" ? Piece.ColorType.White : Piece.ColorType.Black;
            return BoardHelper.ConvertToMoveNotationCoordinates(chessboardGenerator, colorToPlay, randomizedMove.MoveNotation);
        }

        private string GetStockFishMove(string fen)
        {
            return stockfishEngine.GetBestMoveFromFen(fen);
        }

        private async Task<List<ExplorerMove>> GetLichessMovesFromFEN(string fen)
        {
            OpeningExplorer openingExplorer = await LichessRequestHelper.GetOpeningMoves(chessboardGenerator.fenSettings, fen);
            return openingExplorer.Moves.OrderByDescending(x => x.Black + x.White + x.Draws).ToList();
        }

        private List<Move> GetChessableMovesFromFEN(string positionFen)
        {
            var moves = new List<Move>();

            foreach (Variation variation in variationsToPractice)
            {
                for (int i = 0; i < variation.moves.Count; i++)
                {
                    if (variation.moves[i].PositionFen == positionFen)
                        moves.Add(variation.moves[i + 1]);
                }
            }

            return moves;
        }
    }
}
