using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using ChessMemoryApp.Model.File_System;
using ChessMemoryApp.Services;

namespace ChessMemoryApp.Model.CourseMaker
{
    public class CourseLoader
    {
        private readonly Dictionary<string, Course> courses = new();

        public CourseLoader()
        {
            //AddOrUpdateCourse("Lifetime Repertoires: Kan Sicilian", "20) 5.c4, Nc2 Setups", 100043);
        }

        public async Task LoadCoursesFromDatabase()
        {
            if (this.courses.Count > 0)
                return;
            Dictionary<string, Course> courses = await CourseService.GetAll();
            foreach (var course in courses)
                this.courses.TryAdd(course.Key, course.Value);
        }

        public async Task LoadCoursesFromFile()
        {
            HashSet<string> courseFiles = await FileHelper.GetLinesFromFile("Courses/courses.txt");
            var courseCache = new Dictionary<string, Course>();
            foreach (var courseFile in courseFiles)
            {
                if (courseCache.TryGetValue(courseFile, out var course))
                {
                    // Use cached object if available
                    courses.TryAdd(course.Name, course);
                }
                else
                {
                    // Deserialize JSON and cache object
                    string jsonData = await FileHelper.GetContentFromFile("Courses/" + courseFile + ".json");
                    course = JsonConvert.DeserializeObject<Course>(jsonData);
                    course.JsonData = jsonData;
                    courseCache.Add(courseFile, course);
                    courses.TryAdd(course.Name, course);
                }
            }
        }

        public void AddOrUpdateCourse(string courseName, string chapterName, int chessableCourseID)
        {
            Course course;

            if (!File.Exists(GetFilePath(courseName.Replace(":", ""))))
            {
                course = AddCourseFromInputFile(courseName, chapterName, chessableCourseID);
                SaveCourseAsJson(course);
                return;
            }

            course = GetCourseFromFile(courseName);
            course ??= new Course(courseName, chessableCourseID);

            course.AddOrUpdateChapterFromInputFile(chapterName);
            SaveCourseAsJson(course);
        }

        public Dictionary<string, Course> GetCourses()
        {
            return courses;
        }

        public Course GetCourse(string name)
        {
            if (courses.ContainsKey(name))
                return courses[name];

            return null;
        }

        public Course GetCourseFromFile(string courseName)
        {
            string filePath = GetFilePath(courseName.Replace(":", ""));
            string jsonData = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<Course>(jsonData);
        }

        public Course AddCourseFromInputFile(string courseName, string chapterName, int chessableCourseID)
        {
            if (courses.ContainsKey(courseName))
                return null;

            var course = new Course(courseName, chessableCourseID);
            Chapter addedChapter = course.AddOrUpdateChapter(chapterName);
            if (addedChapter == null)
                return null;

            addedChapter.ReadVariatonsFromInputFile();
            courses.Add(courseName, course);

            return course;
        }

        public void SaveCourseAsJson(Course course)
        {
            string jsonData = JsonConvert.SerializeObject(course);
            if (!Directory.Exists("Courses"))
                Directory.CreateDirectory("Courses");

            string filePath = GetFilePath(course.Name);
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            using var sw = new StreamWriter(filePath, false);
            sw.WriteLine(jsonData);
        }

        private string GetFilePath(string courseName)
        {
            return "Courses/" + courseName + ".json";
        }
    }
}
