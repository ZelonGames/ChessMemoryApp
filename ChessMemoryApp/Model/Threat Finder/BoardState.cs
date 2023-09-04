using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class BoardState
    {
        public List<string> threats = new List<string>();
        public readonly string moveNotation;
        public readonly string fen;

        public BoardState(string moveNotation, string fen)
        {
            this.moveNotation = moveNotation; 
            this.fen = fen;
        }
    }
}
