using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string SavePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

    public static void Save<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath(fileName), json);
    }

    public static T Load<T>(string fileName)
    {
        string path = SavePath(fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        return default;
    }
}
