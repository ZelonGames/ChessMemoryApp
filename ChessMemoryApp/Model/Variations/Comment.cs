using ChessMemoryApp.Services;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Variations
{
    public class Comment : ISqLiteService_Comment
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        //[PrimaryKey]
        public string Fen { get; set; }
        public string Text { get; set; }

        public Comment()
        {

        }

        public string GetKey(Comment comment)
        {
            return Fen;
        }
    }
}

