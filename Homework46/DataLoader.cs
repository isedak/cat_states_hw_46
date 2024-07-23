using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Homework46
{
    public static class DataLoader
    {
        public static Cat GetCat(string path)
        {
            string result = File.ReadAllText(path);
            if (result == null || result == "[]")
                throw new FileNotFoundException($"The file is empty.");
            Cat cat = JsonSerializer.Deserialize<Cat>(result) ??
                      throw new FileNotFoundException("The file is invalid.");
            cat.State = Cat.SetState(cat.StateName);
            return cat;
        }

        public static void SaveFile(Cat cat, string path)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(cat, options);
            File.WriteAllText(path, json);
        }
    }
}