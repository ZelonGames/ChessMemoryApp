using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.File_System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.PegList
{
    public class ChessToPegConverter
    {
        public readonly PegCollection pegCollection;
        private readonly Dictionary<char, string> pieceDigits = new()
        {
            { 'K', "1" },
            { 'N', "2" },
            { 'R', "4" },
            { 'Q', "7" },
            { 'B', "9" },
        };

        public ChessToPegConverter(PegCollection pegCollection)
        {
            this.pegCollection = pegCollection;
        }

        public List<string> GetPegListsFromChapter(Chapter chapter)
        {
            var list = new List<string>();

            foreach (var variation in chapter.GetVariations())
            {
                if (variation.Key.Contains("Information"))
                    continue;

                string story = variation.Key + " ";
                foreach (var move in variation.Value.moves)
                {
                    int pegNumber = Convert.ToInt32(GetToCoordinatesAsNumber(move));
                    if (pegCollection.PegDictionary.ContainsKey(pegNumber) && !string.IsNullOrEmpty(pegCollection.PegDictionary[pegNumber].Peg))
                        story += pegCollection.PegDictionary[pegNumber].Peg + " ";
                    else
                        story += pegNumber + " ";
                }

                list.Add(story[..^1]);
            }

            return list;
        }

        public string GetToCoordinatesAsNumber(Move move)
        {
            string moveNotation = move.MoveNotation.Replace("+", "").Replace("#", "").Replace("=", "").Replace("x", "");

            string row = move.Color == Piece.ColorType.White ? "1" : "8";
            if (moveNotation == "O-O")
                return "7" + row;
            else if (moveNotation == "O-O-O")
                return "3" + row;

            return GetColumnAsDigitFromMove(move) + moveNotation.Last();
        }

        public string GetMoveNotationAsNumber(Move move)
        {
            string moveNotation = move.MoveNotation.Replace("+", "").Replace("#", "").Replace("=", "").Replace("x", "");

            string row = move.Color == Piece.ColorType.White ? "1" : "8";
            if (moveNotation == "O-O")
                return pieceDigits['K'] + "7" + row;
            else if (moveNotation == "O-O-O")
                return pieceDigits['K'] + "3" + row;

            bool isPawnMove = char.IsLower(moveNotation[0]);
            if (isPawnMove)
                return GetColumnAsDigitFromMove(move) + moveNotation.Last();
            else
                return GetPieceAsDigitFromMove(move) + GetColumnAsDigitFromMove(move) + moveNotation.Last();
        }

        public string GetPieceAsDigitFromMove(Move move)
        {
            string moveNotation = move.MoveNotation.Replace("x", "").Replace("#", "").Replace("+", "").Replace("=", "");
            char piece = moveNotation.Where(x => char.IsUpper(x) && char.IsLetter(x)).First();
            return pieceDigits[piece];
        }

        public static string GetColumnAsDigitFromMove(Move move)
        {
            string moveNotation = move.MoveNotation.Replace("x", "").Replace("#", "").Replace("+", "").Replace("=", "");
            string columns = "abcdefgh";
            char column = moveNotation.Where(x => char.IsLower(x) && char.IsLetter(x)).Last();
            return (columns.IndexOf(column) + 1).ToString();
        }
    }
}
