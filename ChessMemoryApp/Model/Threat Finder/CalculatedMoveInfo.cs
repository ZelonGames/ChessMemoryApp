using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public readonly struct CalculatedMoveInfo
    {
        public readonly string fen;
        public readonly string moveNotationCoordinates;

        public CalculatedMoveInfo(string fen, string moveNotationCoordinates)
        {
            this.fen = fen;
            this.moveNotationCoordinates = moveNotationCoordinates;
        }
    }
}
