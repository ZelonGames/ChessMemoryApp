using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.File_System
{
    public class FileHelper
    {
        public static void CreateFile(string path)
        {
            string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path);

            using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        }

        public static async Task SaveFile(string path, string content)
        {
            using Stream stream = await FileSystem.Current.OpenAppPackageFileAsync(path);
            using var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(content);
        }

        public static async Task<string> GetContentFromFile(string path)
        {
            using Stream stream = await FileSystem.Current.OpenAppPackageFileAsync(path);
            using var streamReader = new StreamReader(stream);

            const int bufferSize = 4096;
            var buffer = new char[bufferSize];
            var stringBuilder = new StringBuilder();

            int bytesRead;
            while ((bytesRead = await streamReader.ReadAsync(buffer, 0, bufferSize)) > 0)
            {
                stringBuilder.Append(buffer, 0, bytesRead);
            }

            return stringBuilder.ToString();
        }


        public static async Task<HashSet<string>> GetLinesFromFile(string path)
        {
            var lines = new HashSet<string>();

            using Stream stream = await FileSystem.Current.OpenAppPackageFileAsync(path);
            using var streamReader = new StreamReader(stream);

            while (!streamReader.EndOfStream)
                lines.Add(streamReader.ReadLine());

            return lines;
        }

        public static async Task<List<string>> GetLinesFromFileList(string path)
        {
            var lines = new List<string>();

            using Stream stream = await FileSystem.Current.OpenAppPackageFileAsync(path);
            using var streamReader = new StreamReader(stream);

            while (!streamReader.EndOfStream)
                lines.Add(streamReader.ReadLine());

            return lines;
        }
    }
}
