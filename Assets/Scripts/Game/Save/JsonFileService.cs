using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Utility class for reading and writing to files using JSON.
/// </summary>
public class JsonFileService
{
    private readonly JsonSerializerSettings settings = new();

    public JsonFileService()
    {
        settings.Converters.Add(new Vector2JsonConverter());
    }

    /// <summary>
    /// Reads the save from the passed file and deserializes it from JSON.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public SaveObject ReadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string fileContents = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<SaveObject>(fileContents, settings);
        }
        return null;
    }

    /// <summary>
    /// Writes the passed save object to the file, serializing it as JSON.
    /// </summary>
    /// <param name="save"></param>
    /// <param name="filePath"></param>
    public void WriteToFile(SaveObject save, string filePath)
    {
        string jsonString = JsonConvert.SerializeObject(save, settings);
        File.WriteAllText(filePath, jsonString);
    }
}
