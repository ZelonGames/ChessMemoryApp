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
    public class StockfishEngine
    {
        private readonly Process process;
        private readonly StreamWriter streamWriter;
        private List<string> responseLines = new List<string>();
        private readonly string stockfishPath = Path.Combine("C:\\", @"C:\Users\Jonas\OneDrive\Skrivbord\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe");
        private object responseLock = new();
        private TaskCompletionSource<bool> commandCompletion;
        public bool IsThinking { get; private set; }

        public StockfishEngine()
        {
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = stockfishPath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            };

            process.Start();
            streamWriter = process.StandardInput;

            Task.Run(ReadProcessOutput);
            GetOutputLines();
        }

        public async Task<string> GetBestMoveFromFen(string fen)
        {
            var outputs = new List<string>();
            RunCommand("position fen " + fen);
            RunCommand("go depth 25");

            IsThinking = true;
            IsThinking = await commandCompletion.Task;

            outputs = GetOutputLines();
            commandCompletion = null;

            return outputs.Last().Split(' ')[1];
        }

        private void ReadProcessOutput()
        {
            using StreamReader streamReader = process.StandardOutput;

            while (!process.HasExited)
            {
                string line = streamReader.ReadLine();
                if (line == null)
                    continue;

                lock (responseLock)
                {
                    responseLines.Add(line);
                    if (line.Contains("bestmove"))
                        commandCompletion.SetResult(false);
                }
            }
        }

        public void RunCommand(string command)
        {
            if (IsThinking)
                return;

            if (streamWriter.BaseStream.CanWrite)
            {
                commandCompletion = new();
                streamWriter.WriteLine(command);
                streamWriter.Flush();
            }
        }

        public List<string> GetOutputLines()
        {
            var lines = new List<string>(responseLines);

            if (!IsThinking)
                responseLines.Clear();

            return lines;
        }
    }
}
