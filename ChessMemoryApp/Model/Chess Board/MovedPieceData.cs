using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Chess_Board
{
    public readonly struct MovedPieceData
    {
        public readonly Dictionary<string, char> removedPieces;
        public readonly Dictionary<string, char> addedPieces;
        public readonly string fromCoordinates;
        public readonly string toCoordinates;
        public readonly bool updateBoardState;

        public MovedPieceData(Dictionary<string, char> removedPieces, Dictionary<string, char> addedPieces, string fromCoordinates, string toCoordinates, bool updateBoardState)
        {
            this.removedPieces = removedPieces;
            this.addedPieces = addedPieces;
            this.fromCoordinates = fromCoordinates;
            this.toCoordinates = toCoordinates;
            this.updateBoardState = updateBoardState;
        }

        public MovedPieceData GetReversedMovePieceData()
        {
            return new MovedPieceData(addedPieces, removedPieces, toCoordinates, fromCoordinates, updateBoardState);
        }
    }
}
