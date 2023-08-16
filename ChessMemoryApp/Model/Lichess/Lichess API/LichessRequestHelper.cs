using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChessMemoryApp.Model.Lichess.Lichess_API
{
    public static class LichessRequestHelper
    {
        public static bool openingsFromPlayer = false;
        public static string LatestRequestedUrl { get; private set; }
        public static async Task<OpeningExplorer> GetOpeningMoves(FenSettings fenSettings, string fen)
        {
            string fenPart = fen.Split(' ')[0];
            string fenSettingsPart = fenSettings.AppliedFenSettings;
            string url = "https://explorer.lichess.ovh/lichess?variant=standard&speeds=blitz,rapid,classical" +
                      $"&ratings=1400,1600,1800,2000,2200,2500" +
                      $"&fen={fenPart}{fenSettingsPart}";
            if (openingsFromPlayer)
            {
                string playerColor = fenSettings.GetColorToPlaySetting() == "w" ? "black" : "white";
                url = $"https://explorer.lichess.ovh/player?player=DarkGrisen&color={playerColor}&fen={fenPart}{fenSettingsPart}&recentGames=0&variant=standard&speeds=blitz,rapid,classical";
            }

            LatestRequestedUrl = url;

            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<OpeningExplorer>(jsonData);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public static async Task<OpeningExplorer> GetGamesFromPlayer(string username, int amountOfGames)
        {
            string url = $"https://lichess.org/api/games/user/{username}?max={amountOfGames}&perfType=blitz|rapid|classical";


            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<OpeningExplorer>(jsonData);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public static void AnalyseGames()
        {
            
        }
    }
}
