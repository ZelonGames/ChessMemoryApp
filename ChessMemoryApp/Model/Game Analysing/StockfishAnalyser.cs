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

        private DepthAnalysisData depthAnalysisData;
        private TaskCompletionSource<bool> dataReceivedTaskCompletionSource;
        private List<PgnGame> games = new();
        private readonly Process process;
        private readonly StreamWriter streamWriter;
        private string currentFen;
        private string latestCommand;
        private string[] blunderMovesLine;
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
            depthAnalysisData = new DepthAnalysisData(depth);
            this.depth = depth;
            commandEval = "go depth " + depth;
            tasks.Add(commandGetFen, SetFen);
            tasks.Add(commandEval, UpdateDepthAnalysisData);

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
            //r1b3k1/ppp2qp1/2np4/4p3/4P1n1/2NP1Q2/PPP2bP1/R1B2K1R b - - 1 14
            //c6d4
            await GetBlunderAnalysis(
                "rn2kb1r/1bq2ppp/p3pn2/1p4P1/3N3P/2N1Bp2/PPPQ4/2KR1B1R b kq - 0 12",
                "f3f2");
        }

        public async Task<bool> GetBlunderAnalysis(string fen, string moveNotationCoordinates)
        {
            string colorToPlay = fen.Split(' ')[1];
            string playerColor = colorToPlay == "w" ? "b" : "w";

            // Change the fen to what it would be if we made this move
            // because we are asking if the move is a blunder.
            fen = FenHelper.MakeMoveWithCoordinates(fen, moveNotationCoordinates);

            dataReceivedTaskCompletionSource = new TaskCompletionSource<bool>();
            RunCommand($"position fen {fen} moves {moveNotationCoordinates}");
            RunCommand(commandGetFen);
            await dataReceivedTaskCompletionSource.Task;
            depthAnalysisData.currentFen = currentFen;

            dataReceivedTaskCompletionSource = new TaskCompletionSource<bool>();
            RunCommand(commandEval);
            await dataReceivedTaskCompletionSource.Task;

            return true;
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

        private void UpdateDepthAnalysisData(DataReceivedEventArgs e)
        {
            depthAnalysisData.UpdateData(e.Data);
            if (depthAnalysisData.IsCompleted)
                SetTaskCompleted();
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


    }
}
