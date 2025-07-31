using Newtonsoft.Json;
using System;
using System.IO;
namespace RPG_003.DataManagements.Datas.Helper
{
    public static class JsonHelper
    {
        public static string ToJson<T>(this T data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Failed to deserialize JSON data.");
        }
        public static void SaveToFile<T>(this T data, string path)
        {
            File.WriteAllText(path, data.ToJson());
        }

        public static T LoadFromFile<T>(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"JSON file not found at: {path}");

            string json = File.ReadAllText(path);
            return FromJson<T>(json);
        }

    }
}