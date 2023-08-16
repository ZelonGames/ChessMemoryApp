using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Services;
using Newtonsoft.Json;
using SQLite;

namespace ChessMemoryApp.Model.CourseMaker
{
    public class Course : ISqLiteService_Course
    {
        public enum MoveNavigation
        {
            Start,
            Next,
            Current,
            Previous,
            End,
        }

        [JsonIgnore, PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        // SqLite property
        [JsonIgnore]
        public string JsonData { get; set; }

        [JsonProperty]
        private readonly Dictionary<string, Chapter> chapters = new();

        [JsonProperty("previewFen")]
        public string PreviewFen { get; set; }

        [JsonProperty("playAsBlack")]
        public bool PlayAsBlack { get; set; }

        [JsonIgnore]
        public string Name { get => name; set { } }

        [JsonProperty]
        private readonly string name;

        [JsonProperty]
        public readonly int chessableCourseID;

        public Course()
        {

        }
        public Course(string name, int chessableCourseID)
        {
            this.name = name.Replace(":", "");
            this.chessableCourseID = chessableCourseID;
        }

        public string GetChessableLink(string fen)
        {
            string startingHtml = "https://www.chessable.com/course/" + chessableCourseID + "/fen/";
            return startingHtml + fen.Split(' ')[0].Replace('/', ';');
        }

        public Move GetRelativeMove(string fen, MoveNavigation moveNavigation)
        {
            fen = fen.Split(' ')[0];

            Dictionary<string, Variation> variations = GetVariationsAccrossAllChapters(fen, true);

            if (variations.Count == 0)
                return null;

            Variation variation = variations.Values.First();
            var moves = variation.moves.Where(x => x.Fen.Split(' ')[0] == fen);
            if (moves == null)
                return null;

            Move move = moves.First();
            int moveIndex = variation.moves.IndexOf(move);

            switch (moveNavigation)
            {
                case MoveNavigation.Start:
                    moveIndex = 0;
                    break;
                case MoveNavigation.Next:
                    moveIndex++;
                    if (moveIndex >= variation.GetAmountOfMoves())
                        return null;
                    break;
                case MoveNavigation.Current:
                    break;
                case MoveNavigation.Previous:
                    moveIndex--;
                    if (moveIndex < 0)
                        return null;
                    break;
                case MoveNavigation.End:
                    moveIndex = variation.GetAmountOfMoves() - 1;
                    break;
                default:
                    break;
            }

            return variation.moves[moveIndex];
        }

        public Dictionary<string, Variation> GetVariationsAccrossAllChapters(string fen = null, bool getFirstVariation = false)
        {
            fen = fen.Split(' ')[0];

            var variations = new Dictionary<string, Variation>();

            foreach (var chapter in GetChapters())
            {
                foreach (var variation in chapter.Value.GetVariations().Values)
                {
                    if (fen != null)
                    {
                        if (variation.AnyMoveContainsFen(fen))
                        {
                            variations.Add(variation.name, variation);
                            if (getFirstVariation)
                                return variations;
                        }
                    }
                    else
                        variations.Add(variation.name, variation);
                }
            }

            return variations;
        }

        public Chapter AddOrUpdateChapterFromInputFile(string chapterName)
        {
            Chapter addedChapter = AddOrUpdateChapter(chapterName);
            if (addedChapter == null)
                return null;

            addedChapter.ReadVariatonsFromInputFile();

            return addedChapter;
        }

        public Chapter AddOrUpdateChapter(string name)
        {
            if (chapters.ContainsKey(name))
                chapters.Remove(name);

            var chapter = new Chapter(name);
            chapters.Add(name, chapter);

            return chapter;
        }

        public Dictionary<string, Chapter> GetChapters()
        {
            return chapters;
        }

        public Chapter GetChapter(string name)
        {
            if (chapters.ContainsKey(name))
                return chapters[name];

            return null;
        }

        public string GetKey(Course course)
        {
            return course.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Course)
                return false;

            var other = (Course)obj;

            return other.chessableCourseID == chessableCourseID;
        }

        public override int GetHashCode()
        {
            return chessableCourseID.GetHashCode() * 13;
        }
    }
}
