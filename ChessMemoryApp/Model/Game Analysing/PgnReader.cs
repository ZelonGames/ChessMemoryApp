using ChessMemoryApp.Model.File_System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Game_Analysing
{
    public class PgnReader
    {
        public PgnReader()
        {
        }

        public async Task<List<PgnGame>> ReadFromFile(string file)
        {
            List<string> lines = await FileHelper.GetLinesFromFileList(file);

            var pgnGames = new List<PgnGame>();
            PgnGame pgnGame = null;

            foreach (var line in lines)
            {
                bool isLineAttribute = line.Contains("[");
                bool isLineOnMovesList = line.Contains("1.");

                if (isLineAttribute)
                {
                    (string value, string name) = GetContentWithinQuotationMarks(line);

                    bool isLineFirstAttribute = name == "Event";
                    if (isLineFirstAttribute)
                        pgnGame = new PgnGame("DarkGrisen");

                    pgnGame.TrySetProperty(name, value);
                }
                else if (isLineOnMovesList)
                {
                    List<string> moves = ParseMoves(line);
                    pgnGame.moves = moves;
                    pgnGames.Add(pgnGame);
                }
            }

            return pgnGames;
        }

        public List<string> ParseMoves(string input)
        {
            var moves = new List<string>();

            foreach (var move in input.Trim().Split(' '))
            {
                if (move.Contains('.') || move == "0-1" || move == "1-0" || move == "1/2-1/2")
                    continue;

                moves.Add(move);
            }

            return moves;
        }

        public (string value, string name) GetContentWithinQuotationMarks(string input)
        {
            string name = null;

            Match match = Regex.Match(input, @"(?<=\[)[^\s]+(?=\s"")");
            if (match.Success)
                name = match.Value;

            match = Regex.Match(input, "(?<=\").*(?=\")");
            if (match.Success)
                return (match.Value, name);

            return (null, null);
        }
    }

    public class PgnGame
    {
        public List<string> moves = new();
        public string Site { get; private set; }
        public string White { get; private set; }
        public string Black { get; private set; }
        public string Result { get; private set; }

        public bool Lost =>
            playerName == White && Result == "0-1" ||
            playerName == Black && Result == "1-0";

        public readonly string playerName;

        private readonly Dictionary<string, PropertyInfo> properties = new();

        public PgnGame(string playerName)
        {
            Type type = typeof(PgnGame);
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
                this.properties.Add(property.Name, property);

            this.playerName = playerName;
        }

        public void TrySetProperty(string name, string value)
        {
            if (properties.TryGetValue(name, out PropertyInfo property) &&
                !Attribute.IsDefined(property, typeof(PgnIgnore)))
                property.SetValue(this, value);
        }
    }

    public class PgnIgnore : Attribute
    {
    }
}
