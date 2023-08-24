using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;

namespace ChessMemoryApp.Model.CourseMaker
{
    public class Move
    {
        [JsonProperty("moveNotation")]
        public string MoveNotation { get; private set; }

        [JsonIgnore]
        private string fen;
        [JsonProperty("fen")]
        public string Fen { get => fen.Split(' ')[0]; private set => fen = value; }

        [JsonProperty("color")]
        public Piece.ColorType Color { get; private set; }

        public Move()
        {

        }
        public Move(string moveNotation, string fen, Piece.ColorType color)
        {
            Fen = fen;
            MoveNotation = moveNotation;
            Color = color;
        }

        public void TempUpdateFEN(string fen)
        {
            Fen = fen;
        }

        public string UpdateFenAndColor(string fen, int currentMove)
        {
            return Fen = FenHelper.MakeMove(fen, MoveNotation.Replace("+", "").Replace("#", ""));
        }
    }
}
