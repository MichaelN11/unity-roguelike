using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// JSON.NET JsonConverter for serializing Vector2.
/// </summary>
public class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        float x = (float)obj["X"];
        float y = (float)obj["Y"];
        return new Vector2(x, y);
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        JObject jObj = new();
        jObj.Add(new JProperty("X", value.x));
        jObj.Add(new JProperty("Y", value.y));
        jObj.WriteTo(writer);
    }
}
