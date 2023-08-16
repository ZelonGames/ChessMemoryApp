using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Services
{
    public interface ISqLiteService<T>
    {
        public int ID { get; set; }

        public string GetKey(T jsonObject);
    }

    public interface ISqLiteService_CustomVariation : ISqLiteService<CustomVariation>
    {
        public string JsonData { get; set; }
    }

    public interface ISqLiteService_Course : ISqLiteService<Course>
    {
        public string JsonData { get; set; }
        public string Name { get; set; }
    }

    public interface ISqLiteService_Comment : ISqLiteService<Comment>
    {
        public string Fen { get; set; }
        public string Text { get; set; }
    }
}
