using System;
using System.IO;
using UnityEngine;

public static class Persistence
{
    public static string BASE_PATH = UnityEngine.Application.persistentDataPath + Path.DirectorySeparatorChar;
    public static string SETTING_FILE = "Settings";
    public static string TABS_FOLDER = "Tabs" + Path.DirectorySeparatorChar;

    public static void CreateDirectories()
    {
        Debug.Log("Base path: " + BASE_PATH);

        string tabFolder = BASE_PATH + TABS_FOLDER;
        if (!Directory.Exists(tabFolder))
        {
            Directory.CreateDirectory(tabFolder);
        }

        string settingsFile = BASE_PATH + SETTING_FILE + ".txt";
        if (!File.Exists(settingsFile))
        {
            File.CreateText(settingsFile);
        }
    }

    public static void SaveObjectToJson(object pObject, string pSubFolder, string pFileName)
    {
        string path = GetAbsolutePath(pSubFolder, pFileName);
        if (path.Equals(string.Empty))
        {
            return;
        }

        StreamWriter Writer = new StreamWriter(path);
        string json = JsonUtility.ToJson(pObject);
        Writer.Write(json);
        Writer.Flush();
        Writer.Close();

        Debug.Log("Successfully saved object to json file: " + path);
    }

    public static object LoadObjectFromJson(Type pType, string pSubFolder, string pFileName)
    {
        string path = GetAbsolutePath(pSubFolder, pFileName);
        if (path.Equals(string.Empty))
        {
            return null;
        }

        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson(json, pType);
    }

    private static string GetAbsolutePath(string pSubFolder, string pFileName)
    {
        string relative = pFileName;
        if (relative.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            Debug.LogError("Detected invalid filename characters! Cannot save with the name: " + relative);
            return string.Empty;
        }

        if (relative.Contains("."))
        {
            string extension = relative.Substring(relative.IndexOf("."));
            if (!extension.Equals(".txt"))
            {
                Debug.LogWarning("Detected a file extension in path. Replacing \"" + extension + "\"with \".txt\"");
            }
            relative = relative.Substring(0, relative.IndexOf("."));
        }

        string path = BASE_PATH + pSubFolder + relative;
        if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        {
            Debug.LogError("Detected invalid path characters! Cannot save to the path: " + path);
            return string.Empty;
        }

        return path + ".txt";
    }

    public static string[] GetFilesInDirectory(string pRelative)
    {
       return Directory.GetFiles(BASE_PATH + pRelative);
    }

}
