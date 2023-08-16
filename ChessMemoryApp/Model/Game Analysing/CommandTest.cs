using ChessMemoryApp.Model.CourseMaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Game_Analysing
{
    internal class CommandTest
    {
        private readonly Process process;
        private readonly StreamWriter streamWriter;
        private List<string> responseLines = new List<string>();
        private readonly string stockfishPath = Path.Combine("C:\\", @"Users\Jonas\Desktop\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe");
        private object responseLock = new object();

        public CommandTest()
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

            // Initialize the process and send initial commands
            RunCommand("position fen " + FenHelper.STARTING_FEN + " moves e2e4 e7e5");
            var a = GetOutputLines();
            RunCommand("d");
            var b = GetOutputLines();
            var c = GetOutputLines();
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
                        {
                            responseLines.Add(line);
                        }
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
                List<string> lines = new List<string>(responseLines);
                responseLines.Clear();
                return lines;
            }
        }

    }
}
