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
                    CreateNoWindow = true
                },
            };

            process.Start();
            streamWriter = process.StandardInput;

            Task.Run(ReadProcessOutput);
        }

        public string GetBestMoveFromFen(string fen)
        {
            RunCommand("position fen " + fen);
            RunCommand("go depth 25");
            List<string> outputs = GetOutputLines();
            return outputs.Last().Split(' ')[1];
        }

        private void ReadProcessOutput()
        {
            using (StreamReader streamReader = process.StandardOutput)
            {
                while (!process.HasExited)
                {
                    string line = streamReader.ReadLine();
                    if (line != null)
                    {
                        lock (responseLock)
                            responseLines.Add(line);
                    }
                }
            }
        }

        public void RunCommand(string command)
        {
            lock (responseLock)
            {
                if (streamWriter.BaseStream.CanWrite)
                {
                    streamWriter.WriteLine(command);
                    streamWriter.Flush();
                }
            }
        }

        public List<string> GetOutputLines()
        {
            lock (responseLock)
            {
                var lines = new List<string>(responseLines);
                responseLines.Clear();
                return lines;
            }
        }
    }
}
