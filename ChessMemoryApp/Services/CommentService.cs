using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Services
{
    public class CommentService : SqLiteService<Comment>
    {
        public static async new Task Add(Comment comment)
        {
            await CreateDatabase();
            await DB.InsertAsync(comment);
        }

        public static async Task Update(Comment comment)
        {
            await CreateDatabase();
            await DB.UpdateAsync(comment);
        }

        public static async new Task<Comment> Get(Comment item)
        {
            await CreateDatabase();
            return await DB.Table<Comment>().Where(x => x.Fen == item.Fen).FirstOrDefaultAsync();
        }

        public static async Task RemoveAll()
        {
            await CreateDatabase();
            await DB.Table<Comment>().DeleteAsync();
        }

        public static async Task Remove(Comment comment)
        {
            if (comment == null)
                return;

            await CreateDatabase();
            await DB.Table<Comment>().DeleteAsync(x => x.Fen == comment.Fen);
        }
        public static async Task<Dictionary<string, Comment>> GetAll()
        {
            await CreateDatabase();

            var jsonObjects = new Dictionary<string, Comment>();
            var objects = await DB.Table<Comment>().ToListAsync();

            foreach (var _object in objects)
            {
                jsonObjects.Add(_object.GetKey(_object), _object);
            }

            return jsonObjects;
        }

        public static async Task<Comment> Get(string fen)
        {
            await CreateDatabase();
            return await DB.Table<Comment>().Where(x => fen == x.Fen).FirstOrDefaultAsync();
        }
    }
}
