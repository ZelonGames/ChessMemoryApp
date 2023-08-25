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
    public class CustomVariationService : SqLiteService<CustomVariation>
    {
        public static async new Task Add(CustomVariation jsonObject)
        {
            await CreateDatabase();
            string jsonData = JsonConvert.SerializeObject(jsonObject);
            jsonObject.JsonData = jsonData;
            if (await Get(jsonObject) != null)
                await DB.UpdateAsync(jsonObject);
            else
                await DB.InsertAsync(jsonObject);
        }

        public static async Task Update(CustomVariation customVariation)
        {
            await CreateDatabase();
            customVariation.JsonData = JsonConvert.SerializeObject(customVariation);
            await DB.UpdateAsync(customVariation);
        }

        public static async Task<Dictionary<string, CustomVariation>> GetAllFromCourse(Course course)
        {
            await CreateDatabase();

            var jsonObjects = new Dictionary<string, CustomVariation>();
            var objects = await DB.Table<CustomVariation>().ToListAsync();

            foreach (var _object in objects.Where(x => x.CourseName == course.Name))
            {
                if (_object.JsonData == null)
                    continue;

                var jsonObject = JsonConvert.DeserializeObject<CustomVariation>(_object.JsonData);
                jsonObject.ID = _object.ID;
                jsonObject.CourseName = _object.CourseName;
                jsonObjects.Add(_object.GetKey(jsonObject), jsonObject);
            }

            return jsonObjects;
        }

        public static async Task Remove(CustomVariation customVariation)
        {
            await CreateDatabase();
            await DB.Table<CustomVariation>().DeleteAsync(x => x.ID == customVariation.ID);
        }
    }
}
