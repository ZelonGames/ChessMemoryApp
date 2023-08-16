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
    public class SqLiteService<T> where T : class, ISqLiteService<T>, new()
    {
        public static SQLiteAsyncConnection DB { get; private set; }

        private readonly static HashSet<string> tables = new();

        public static async Task CreateDatabase()
        {
            if (DB == null)
            {
                string databasePath = Path.Combine(FileSystem.AppDataDirectory, "ChessMemory.db");
                DB = new SQLiteAsyncConnection(databasePath);
            }

            string tableName = GetTableName();
            if (!tables.Contains(tableName))
            {
                tables.Add(tableName);
                await DB.CreateTableAsync<T>();
            }
        }

        public static string GetTableName()
        {
            return typeof(T).Name;
        }

        public static async Task Add(T item)
        {
            await CreateDatabase();
            if (await Get(item) != null)
                await DB.UpdateAsync(item);
            else
                await DB.InsertAsync(item);
        }

        public static async Task<T> Get(T item)
        {
            await CreateDatabase();
            return await DB.Table<T>().Where(x => x.ID == item.ID).FirstOrDefaultAsync();
        }
    }
}
