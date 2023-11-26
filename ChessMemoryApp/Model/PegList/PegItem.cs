using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.PegList
{
    public class PegItem
    {
        [JsonProperty("number")]
        public int Number { get; private set; }

        [JsonProperty("peg")]
        public string Peg { get; private set; }

        public PegItem() 
        {
            //Peg = Peg.Replace("--", "").Replace("*", "").Replace("?", "");
        }
    }
}
