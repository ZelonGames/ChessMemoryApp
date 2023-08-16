using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChessMemoryApp.Model.Lichess.Lichess_API
{
    public class OpeningExplorer
    {
        [JsonProperty("white")]
        public int White { get; private set; }

        [JsonProperty("black")]
        public int Black { get; private set; }

        [JsonProperty("draws")]
        public int Draws { get; private set; }

        [JsonProperty("moves")]
        public List<ExplorerMove> Moves { get; private set; }
    }

    public class ExplorerMove
    {
        [JsonProperty("averageRating")]
        public int AverageRating { get; private set; }

        [JsonProperty("white")]
        public int White { get; private set; }

        [JsonProperty("black")]
        public int Black { get; private set; }

        [JsonProperty("draws")]
        public int Draws { get; private set; }
        
        [JsonProperty("san")]
        public string MoveNotation { get; private set; }

        [JsonProperty("uci")]
        public string MoveNotationCoordinates { get; private set; }

        public int TotalGames => White + Black + Draws;

        public string GetWinsInPercent(bool playAsBlack)
        {
            float wins = playAsBlack ? Black : White;
            double percentValue = MathF.Round(wins / TotalGames * 100, 0);

            return percentValue + "%";
        }
    }
}
