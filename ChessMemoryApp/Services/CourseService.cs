using ChessMemoryApp.Model.CourseMaker;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Services
{
    public class CourseService : SqLiteService<Course>
    {
        public static async Task RemoveAll()
        {
            await CreateDatabase();
            var rows = await GetAll();
            foreach (var row in rows)
                await DB.Table<Course>().DeleteAsync(x => x.Name == row.Value.Name);
        }


        public static async Task Update(Course course)
        {
            await CreateDatabase();
            await DB.UpdateAsync(course);
        }

        public static async new Task<Course> Get(Course item)
        {
            await CreateDatabase();
            return await DB.Table<Course>().Where(x => x.Name == item.Name).FirstOrDefaultAsync();
        }

        public static async Task<Dictionary<string, Course>> GetAll()
        {
            await CreateDatabase();

            var jsonObjects = new Dictionary<string, Course>();
            var objects = await DB.Table<Course>().ToListAsync();

            foreach (var _object in objects)
            {
                if (_object.JsonData == null)
                {
                    
                    continue;
                }

                var jsonObject = JsonConvert.DeserializeObject<Course>(_object.JsonData);
                jsonObject.ID = _object.ID;
                jsonObject.Name = _object.Name;
                jsonObject.PreviewFen = _object.PreviewFen;
                jsonObject.PlayAsBlack = _object.PlayAsBlack;

                if (!jsonObjects.ContainsKey(jsonObject.Name))
                    jsonObjects.Add(_object.GetKey(jsonObject), jsonObject);
                else
                {
                    jsonObjects[jsonObject.Name].PreviewFen = _object.PreviewFen;
                    jsonObjects[jsonObject.Name].PlayAsBlack = _object.PlayAsBlack;
                }
            }

            return jsonObjects;
        }

        public static string GetKey(Course jsonObject)
        {
            return jsonObject.Name;
        }
    }
}
