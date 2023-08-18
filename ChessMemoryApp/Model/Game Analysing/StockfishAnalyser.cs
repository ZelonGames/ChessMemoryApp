using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Game_Analysing
{
    public class StockfishAnalyser
    {
        private readonly string stockfishPath = Path.Combine("C:\\",
            @"Users\Jonas\Desktop\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe");

        private TaskCompletionSource<bool> dataReceivedTaskCompletionSource;
        private List<PgnGame> games = new();
        private readonly Process process;
        private readonly StreamWriter streamWriter;
        private string currentFen;
        private string latestCommand;
        private string[] blunderMovesLine;
        private List<string[]> linesForEachDepth;
        private List<float> evaluationsForEachDepth;
        private float currentEvaluation;
        private readonly int depth;
        private readonly int blunderDepth = 3;
        private const float BLUNDER_VALUE = 1f;

        #region commands
        private readonly string commandEval;
        private readonly string commandGetFen = "d";
        #endregion

        private readonly Dictionary<string, Action<DataReceivedEventArgs>> tasks = new();

        public StockfishAnalyser(int depth)
        {
            this.depth = depth;
            commandEval = "go depth " + depth;
            tasks.Add(commandGetFen, SetFen);
            tasks.Add(commandEval, AnalysePosition);

            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = stockfishPath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
            };

            process.Start();
            process.BeginOutputReadLine();

            process.OutputDataReceived += Process_OutputDataReceived;
            streamWriter = process.StandardInput;

            Test();
        }

        public async void Test()
        {
            await GetBlunderAnalysis(
                "2r5/p1r2kpp/1p3p2/2PPp3/3pP3/NP5P/P2n2P1/2R1R2K w - - 3 29",
                "c5b6");
        }

        public async Task<bool> GetBlunderAnalysis(string fen, string moveNotationCoordinates)
        {
            string colorToPlay = fen.Split(' ')[1];
            string playerColor = colorToPlay == "w" ? "b" : "w";

            // Change the fen to what it would be if we made this move
            // because we are asking if the move is a blunder.
            fen = FenHelper.MakeMove(fen, moveNotationCoordinates);

            dataReceivedTaskCompletionSource = new TaskCompletionSource<bool>();
            RunCommand($"position fen {fen} moves {moveNotationCoordinates}");
            RunCommand(commandGetFen);
            await dataReceivedTaskCompletionSource.Task;

            dataReceivedTaskCompletionSource = new TaskCompletionSource<bool>();
            RunCommand(commandEval);
            await dataReceivedTaskCompletionSource.Task;

            return true;
        }

        public async Task AnalyseGames(string pgnFile)
        {
            PgnReader reader = new PgnReader();
            List<PgnGame> blunderGames = new();
            List<PgnGame> games = await reader.ReadFromFile(pgnFile);

            foreach (var game in games)
            {
                if (!game.Lost)
                    continue;

                currentFen = FenHelper.STARTING_FEN;
                float previousEvaluation = 0;

                currentFen = "2b2rk1/p1n1q1pp/2pp1r2/2p1p3/P4P1Q/3PNNP1/1PP4P/R4RK1 w - - 0 20";

                for (int i = 0; i < game.moves.Count; i++)
                {
                    string moveNotation = game.moves[i];
                    var color = i % 2 == 0 ? Piece.ColorType.White : Piece.ColorType.Black;
                    string moveNotationCoordinates = FenHelper.ConvertToMoveNotationCoordinates(currentFen, moveNotation, color);

                    dataReceivedTaskCompletionSource = new TaskCompletionSource<bool>();
                    RunCommand($"position fen {currentFen} moves {moveNotationCoordinates}");
                    RunCommand(commandGetFen);
                    await dataReceivedTaskCompletionSource.Task;

                    dataReceivedTaskCompletionSource = new TaskCompletionSource<bool>();
                    RunCommand(commandEval);
                    await dataReceivedTaskCompletionSource.Task;

                    if (IsPositionWorseThanOpponent(game))
                    {
                        if (IsMoveBlunder(game, previousEvaluation))
                        {
                            string analysingFen = currentFen;

                            var lostPieces = new List<char>();
                            var recapturedPieces = new List<char>();

                            for (int m = 0; m < blunderMovesLine.Length; m++)
                            {
                                string toCoordinate = blunderMovesLine[m][2..];
                                char? piece = FenHelper.GetPieceOnSquare(analysingFen, toCoordinate);

                                if (piece.HasValue)
                                {
                                    bool isYourOwnPieceCaptured =
                                        char.IsLower(piece.Value) && game.playerName == game.Black ||
                                        char.IsUpper(piece.Value) && game.playerName == game.White;

                                    if (isYourOwnPieceCaptured)
                                        lostPieces.Add(piece.Value);
                                    else
                                        recapturedPieces.Add(piece.Value);
                                }

                                analysingFen = FenHelper.MakeMove(analysingFen, blunderMovesLine[m]);
                            }
                        }
                    }

                    previousEvaluation = currentEvaluation;
                }
            }
        }


        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) ||
                dataReceivedTaskCompletionSource == null ||
                string.IsNullOrEmpty(latestCommand))
                return;


            if (tasks.ContainsKey(latestCommand))
                tasks[latestCommand].Invoke(e);
        }

        private void RunCommand(string command)
        {
            if (streamWriter.BaseStream.CanWrite)
            {
                latestCommand = command;
                streamWriter.WriteLine(command);
                streamWriter.Flush();
            }
        }

        private void InvokeMethodByName(string methodName, params object[] arguments)
        {
            MethodInfo method = GetType().GetMethod(methodName);

            if (method != null)
                method.Invoke(this, arguments);
        }

        private void AnalysePosition(DataReceivedEventArgs e)
        {
            int currentDepth = GetDepth(e.Data);
            if (currentDepth == 0)
                return;

            if (currentDepth == 1)
            {
                linesForEachDepth = new List<string[]>();
                evaluationsForEachDepth = new List<float>();
            }

            string[] line = GetLine(e.Data);
            linesForEachDepth.Add(line);

            int centiPawns = GetCentiPawns(e.Data);
            float evaluation = ConvertCentipawnsToEvaluation(currentFen, centiPawns);
            evaluationsForEachDepth.Add(evaluation);

            if (currentDepth == depth)
            {
                currentEvaluation = evaluation;
                SetTaskCompleted();
            }
        }

        private void SetFen(DataReceivedEventArgs e)
        {
            if (e.Data.Contains("Fen"))
            {
                currentFen = e.Data.Replace("Fen: ", "");
                SetTaskCompleted();
            }
        }

        private void SetTaskCompleted()
        {
            if (dataReceivedTaskCompletionSource != null &&
                !dataReceivedTaskCompletionSource.Task.IsCompleted)
            {
                latestCommand = null;
                dataReceivedTaskCompletionSource.SetResult(true);
            }
        }

        private string[] GetLine(string line)
        {
            if (!line.Contains(" pv "))
                return null;

            string pattern = @" pv (.*)";
            MatchCollection matches = Regex.Matches(line, pattern);

            return matches[0].Groups[1].Value.Split(' ');

        }

        private float ConvertCentipawnsToEvaluation(string fen, int centipawns)
        {
            string colorToPlay = fen.Split(' ').Where(x => x == "b" || x == "w").First();

            if (colorToPlay == "b")
                centipawns *= -1;

            return centipawns / 100f;
        }

        private int GetDepth(string line)
        {
            if (!line.Contains("depth"))
                return 0;

            string pattern = @"info depth\s*(\d+)";
            MatchCollection matches = Regex.Matches(line, pattern);

            return Convert.ToInt32(matches.First().Groups[1].Value);
        }

        private int GetCentiPawns(string line)
        {
            if (!line.Contains("score cp"))
                return 0;

            string pattern = @"score cp\s*([-+]?\d+)";
            MatchCollection matches = Regex.Matches(line, pattern);
            return Convert.ToInt32(matches.First().Groups[1].Value.Replace("+", ""));
        }

        private bool IsPositionWorseThanOpponent(PgnGame game)
        {
            return game.playerName == game.White && currentEvaluation < 0
                || game.playerName == game.Black && currentEvaluation > 0;
        }

        private bool IsMoveBlunder(PgnGame game, float previousEvaluation)
        {
            return
                game.playerName == game.White && currentEvaluation < previousEvaluation - BLUNDER_VALUE ||
                game.playerName == game.Black && currentEvaluation > previousEvaluation + BLUNDER_VALUE;
        }
    }
}
