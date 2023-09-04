using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class AttackedPiece
    {
        public int TimesAttacked => threateningPieces.Count;
        public readonly char pieceType;
        public Dictionary<string, char> threateningPieces = new();

        public AttackedPiece(char pieceType)
        {
            this.pieceType = pieceType;
        }

        public void TryAddThreateningPiece(string pieceCoordinates, char pieceType)
        {
            threateningPieces.TryAdd(pieceCoordinates, pieceType);
        }
    }
}
