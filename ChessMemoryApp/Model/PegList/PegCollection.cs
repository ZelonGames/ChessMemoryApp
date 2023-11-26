using ChessMemoryApp.Model.File_System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.PegList
{
    public class PegCollection
    {
        [JsonProperty("pegs")]
        public List<PegItem> Pegs { get; private set; } = new();

        [JsonIgnore]
        public Dictionary<int, PegItem> PegDictionary { get; private set; } = new();

        public static async Task<PegCollection> LoadPegCollection()
        {
            string jsonData = await FileHelper.GetContentFromFile("MemoryPalace/chessPeglist.json");
            var pegCollection = JsonConvert.DeserializeObject<PegCollection>(jsonData);

            foreach (var peg in pegCollection.Pegs)
                pegCollection.PegDictionary.Add(peg.Number, peg);

            return pegCollection;
        }
    }
}
