using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChessMemoryApp.Model.CourseMaker
{
    public class Chapter
    {
        [JsonProperty("variations")]
        private readonly Dictionary<string, Variation> variations = new Dictionary<string, Variation>();

        public readonly string name;

        public Chapter()
        {

        }
        public Chapter(string name)
        {
            this.name = name.Replace(":", "");
        }

        public void ReadVariatonsFromInputFile()
        {
            string variationName = "";

            using (var sr = new StreamReader("Input/input.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string lineContent = sr.ReadLine().Replace("informational", "").Replace("alternative", "");

                    if (lineContent.Contains("IMPORTANT") ||
                        lineContent.Contains("Overstudy") ||
                        lineContent.Contains("View") ||
                        CourseHelper.EndsWithNumbers(lineContent, "Review moves") ||
                        CourseHelper.EndsWithNumbers(lineContent, "Learn moves") ||
                        CourseHelper.ContainsSquareBrackets(lineContent))
                        continue;

                    if (CourseHelper.BeginsWithDigitsAndDot(lineContent))
                    {
                        if (variationName.Length == 0)
                        {
                            // In case the variation name contains the same criteria as the move notations
                            variationName = lineContent;
                            continue;
                        }
                        if (variationName[0] == '[')
                            continue;

                        if (variationName == "Review moves (5)Overstudy")
                        {

                        }

                        Variation addedVariation = AddVariation(variationName);
                        variationName = "";
                        string[] separatedMoves = lineContent.Split(' ');

                        foreach (var separatedMove in separatedMoves)
                        {
                            string moveNotation = separatedMove;
                            if (moveNotation.Contains("."))
                                moveNotation = separatedMove.Split('.')[1];

                            addedVariation.AddMove(moveNotation);
                        }
                    }
                    else // The variation name is always the line before the line that contains the move notations
                        variationName = lineContent;
                }
            }
        }

        public Dictionary<string, Variation> GetVariations()
        {
            return variations;
        }

        public Variation GetVariation(string name)
        {
            if (variations.ContainsKey(name))
                return variations[name];

            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>returns the added variation</returns>
        private Variation AddVariation(string name)
        {
            if (name == "5.Bd3 Bc5 6.Be3 d6")
            {

            }
            if (variations.ContainsKey(name))
            {
                string[] nameComponents = name.Split(' ');
                bool hasNameIdentifier = nameComponents.Length > 0 && nameComponents.Last().Length == 1 && char.IsUpper(nameComponents.Last()[0]);
                if (hasNameIdentifier)
                {
                    char identifier = name.Split(' ').Last()[0];
                    name = name.Replace(" " + identifier, "");
                    name += " " + CourseHelper.GetNextLetter(identifier);
                }
                else
                    name += " B";
            }

            var variation = new Variation(name);
            variations.Add(name, variation);
            return variation;
        }
    }
}
