using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public readonly struct TeleportedPieceResult
    {
        public readonly Dictionary<string, char> removedPieces;
        public readonly Dictionary<string, char> addedPieces;
        public readonly bool madeMove;

        public TeleportedPieceResult(Dictionary<string, char> removedPieces, Dictionary<string, char> addedPieces, bool madeMove)
        {
            this.removedPieces = removedPieces;
            this.addedPieces = addedPieces;
            this.madeMove = madeMove;
        }

        public TeleportedPieceResult()
        {
        }
    }
}
