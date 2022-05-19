using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutingAppBase.Event
{
  public static class JsonUtil
  {
    private static readonly string ConfigFileName = "config.json";
    private static readonly string MatchFilePrefix = "match-";

    private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true
    };

    /// <summary>
    /// Load the last saved event from the given folder.
    /// </summary>
    /// <returns>The Event if loading was successful, null if something went wrong</returns>
    public static EventData? LoadEvent(string storageFolder)
    {
      var eventConfig = LoadConfig(Path.Combine(storageFolder, ConfigFileName));
      if (eventConfig == null)
      {
        return null;
      }

      var matches = new List<MatchData>();

      foreach (var file in Directory.EnumerateFiles(storageFolder))
      {
        if (file.StartsWith(MatchFilePrefix) && file.EndsWith(".json"))
        {
          var match = Deserialize<MatchData>(Path.Combine(file));
          if (match != null) matches.Add(match);
        }
      }

      return new EventData(eventConfig, matches);
    }

    /// <summary>
    /// Save an event in the given folder. Previously saved matches are deleted first.
    /// </summary>
    /// <param name="ev"></param>
    /// <param name="storageFolder"></param>
    public static void SaveEvent(EventData ev, string storageFolder)
    {
      Serialize(ev.Config, Path.Combine(storageFolder, ConfigFileName));

      // Delete all the previous matches
      foreach (var file in Directory.EnumerateFiles(storageFolder))
      {
        if (file.StartsWith(MatchFilePrefix) && file.EndsWith(".json"))
        {
          File.Delete(file);
        }
      }

      var matches = new List<MatchData>();

      int i = 0;
      foreach (var match in ev.Matches)
      {
        var matchName = match.Id ?? i.ToString();
        Serialize(match, Path.Combine(storageFolder, $"{MatchFilePrefix}{matchName}.json"));
        i++;
      }
    }

    /// <summary>
    /// Try to deserialize an EventConfig from JSON. Returns null if it was invalid.
    /// </summary>
    /// <param name="path">The config path</param>
    public static EventConfig? LoadConfig(string path)
    {
      var config = Deserialize<EventConfig>(path);
      if (config == null) return null;

      // todo more complete validation
      if (config.EventName == null || config.FieldConfigs == null) return null;

      foreach (var field in config.FieldConfigs)
      {
        if (field.Name == null) return null;

        if (field.Type == FieldType.Radio && field.Choices == null) return null;
      }

      return config;
    }

    private static void Serialize<T>(T obj, string path)
    {
      var json = JsonSerializer.Serialize<T>(obj, jsonOptions);
      File.WriteAllText(path, json);
    }

    private static T? Deserialize<T>(string path) where T : class
    {
      var json = File.ReadAllText(path);
      return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }
  }
}
