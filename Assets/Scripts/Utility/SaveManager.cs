using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using System.IO;

public static class SerializationIOHandler
{
    private const string m_saveFolder = "/appdata/";

    public static T Load<T>(string filename)
    {
        string filePath = GetFullFilePath(filename);

        if (!File.Exists(filePath))
            return default(T);

        byte[] bytes = File.ReadAllBytes(filePath);
        return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
    }

    public static void Save<T>(string filename, T data)
    {
        Directory.CreateDirectory(GetFullFilePath(""));
        string filePath = GetFullFilePath(filename);
        byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);
        File.WriteAllBytes(filePath, bytes);
    }

    private static string GetFullFilePath(string filename)
    {
        #if UNITY_WEBGL
                return Application.persistentDataPath + m_saveFolder + filename;
        #else
                return Application.dataPath + m_saveFolder + filename;
        #endif
    }
}
