using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Game_Analysing
{
    /// <summary>
    /// Returns the data you get from "go depth x" command
    /// </summary>
    public class DepthAnalysisData
    {
        public List<string[]> linesForEachDepth;
        public List<string> forcingLine;
        public List<float> evaluationsForEachDepth;

        public string currentFen;
        private int depth;

        public bool IsCompleted { get; private set; }

        public DepthAnalysisData(int depth)
        {
            this.depth = depth;
        }

        public void UpdateData(string commandOutputLine)
        {
            int currentDepth = GetDepth(commandOutputLine);
            if (currentDepth == 0)
                return;

            if (currentDepth == 1)
            {
                linesForEachDepth = new List<string[]>();
                evaluationsForEachDepth = new List<float>();
            }

            string[] line = GetChessMoves(commandOutputLine);
            linesForEachDepth.Add(line);

            int centiPawns = GetCentiPawns(commandOutputLine);
            float evaluation = ConvertCentipawnsToEvaluation(currentFen, centiPawns);
            evaluationsForEachDepth.Add(evaluation);

            if (currentDepth == depth)
            {
                IsCompleted = true;
                var value = GetCapturedPieceValue();
            }
        }

        public (int capturedWhitePiecesValue, int capturedBlackPiecesValue) GetCapturedPieceValue()
        {
            var deepestLine = linesForEachDepth.Last();
            string colorToPlay = FenHelper.GetColorToPlayFromFen(currentFen);

            var capturedWhitePieces = new List<char>();
            var capturedBlackPieces = new List<char>();

            string lineFen = currentFen;

            int plyMovesSinceLastCapturedPiece = 0;

            for (int i = 0; i < deepestLine.Length; i++)
            {
                var currentMoveNotationCoordinates = deepestLine[i];
                string toCoordinates = BoardHelper.GetToCoordinatesString(currentMoveNotationCoordinates);
                char? capturedPiece = FenHelper.GetPieceOnSquare(lineFen, toCoordinates);

                plyMovesSinceLastCapturedPiece++;

                if (capturedPiece.HasValue)
                {
                    if (colorToPlay == "w")
                        capturedBlackPieces.Add(capturedPiece.Value);
                    else
                        capturedWhitePieces.Add(capturedPiece.Value);

                    forcingLine.Add(currentMoveNotationCoordinates);
                    plyMovesSinceLastCapturedPiece = 0;
                }

                lineFen = FenHelper.MakeMoveWithCoordinates(lineFen, currentMoveNotationCoordinates);
                colorToPlay = FenHelper.GetColorToPlayFromFen(lineFen);
            }

            int capturedWhitePiecesValue = CalculatePieceValues(capturedWhitePieces);
            int capturedBlackPiecesValue = CalculatePieceValues(capturedBlackPieces);

            return (capturedWhitePiecesValue, capturedBlackPiecesValue);
        }

        private int GetDepth(string commandOutputLine)
        {
            if (!commandOutputLine.Contains("depth"))
                return 0;

            string pattern = @"info depth\s*(\d+)";
            MatchCollection matches = Regex.Matches(commandOutputLine, pattern);

            return Convert.ToInt32(matches.First().Groups[1].Value);
        }

        private string[] GetChessMoves(string commandOutputLine)
        {
            if (!commandOutputLine.Contains(" pv "))
                return null;

            string pattern = @" pv (.*)";
            MatchCollection matches = Regex.Matches(commandOutputLine, pattern);

            return matches[0].Groups[1].Value.Split(' ');
        }

        private float ConvertCentipawnsToEvaluation(string fen, int centipawns)
        {
            string colorToPlay = fen.Split(' ').Where(x => x == "b" || x == "w").First();

            if (colorToPlay == "b")
                centipawns *= -1;

            return centipawns / 100f;
        }

        private int GetCentiPawns(string line)
        {
            if (!line.Contains("score cp"))
                return 0;

            string pattern = @"score cp\s*([-+]?\d+)";
            MatchCollection matches = Regex.Matches(line, pattern);
            return Convert.ToInt32(matches.First().Groups[1].Value.Replace("+", ""));
        }

        private int CalculatePieceValues(List<char> pieces)
        {
            var pieceValues = new Dictionary<char, int>()
            {
                { 'p', 1 },
                { 'b', 3 },
                { 'n', 3 },
                { 'r', 5 },
                { 'q', 9 },
            };

            return pieces.Sum(x => pieceValues[char.ToLower(x)]);
        }
    }
}
