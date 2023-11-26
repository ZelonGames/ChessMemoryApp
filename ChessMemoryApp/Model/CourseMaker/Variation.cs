using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;
using ChessMemoryApp.Model.Chess_Board.Pieces;

namespace ChessMemoryApp.Model.CourseMaker
{
    public class Variation
    {
        [JsonProperty("moves")]
        public readonly List<Move> moves = new List<Move>();

        [JsonProperty("name")]
        public readonly string name;

        public Variation()
        {

        }
        public Variation(string name)
        {
            this.name = name;
        }

        public int GetAmountOfMoves()
        {
            return moves.Count;
        }

        public bool AnyMoveContainsFen(string fen)
        {
            return moves.Any(y => y.Fen == fen);
        }

        public void AddMove(string moveNotation)
        {
            moves.Add(new Move(moveNotation, "", moves.Count % 2 == 0 ? Piece.ColorType.White : Piece.ColorType.Black));
        }

        public Move GetMove(int move)
        {
            if (move >= moves.Count)
                return null;

            return moves[move];
        }
    }
}
