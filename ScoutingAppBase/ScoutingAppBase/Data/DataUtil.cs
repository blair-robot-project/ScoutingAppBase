using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ScoutingAppBase.Data
{
  public static class DataUtil
  {
    private const string ConfigFileName = "config.json";
    private const string MatchFilePrefix = "match-";

    /// <summary>
    /// Regex to check if a file could be a serialized match
    /// </summary>
    private static readonly Regex MatchFileRegex = new Regex($"{MatchFilePrefix}\\d+.json");

    /// <summary>
    /// The folder where all event data will be stored
    /// </summary>
    private static readonly string StorageFolder =
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)},
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true
    };

    /// <summary>
    /// Load the last saved event from the given folder.
    /// </summary>
    /// <returns>The Event if loading was successful, null if something went wrong</returns>
    public static EventData? LoadEvent()
    {
      var eventConfig = LoadConfig(Path.Combine(StorageFolder, ConfigFileName));
      if (eventConfig == null) return null;

      var matches =
        from file in Directory.EnumerateFiles(StorageFolder)
        where file.StartsWith(MatchFilePrefix) && file.EndsWith(".json")
        let match = Deserialize<MatchData>(Path.Combine(file))
        where match != null
        select (MatchData) match;

      return new EventData(eventConfig, matches);
    }

    /// <summary>
    /// Save an event in the given folder. Previously saved matches are deleted first.
    /// </summary>
    /// <param name="ev"></param>
    public static void SaveEvent(EventData ev)
    {
      Serialize(ev.Config, Path.Combine(StorageFolder, ConfigFileName));

      // Delete all the previous matches
      foreach (string file in Directory.EnumerateFiles(StorageFolder))
      {
        if (MatchFileRegex.IsMatch(file)) File.Delete(file);
      }

      ev.Matches.ForEach(SaveMatch);
    }

    public static void SaveMatch(MatchData match) => Serialize(match,
      Path.Combine(StorageFolder, $"{MatchFilePrefix}{match[GeneralFields.MatchNum]}.json"));

    /// <summary>
    /// Try to deserialize an EventConfig from JSON. Returns null if it was invalid.
    /// </summary>
    /// <param name="path">The config path</param>
    public static EventConfig? LoadConfig(string path)
    {
      var config = Deserialize<EventConfig>(path);
      if (config == null) return null;

      // todo more complete validation
      if (config.EventName == null || config.SpecFieldConfigs == null) return null;

      foreach (var field in config.SpecFieldConfigs)
      {
        if (field.Name == null) return null;

        if (field.Type == FieldType.Choice && field.Choices == null) return null;
      }

      return config;
    }

    private static void Serialize<T>(T obj, string path) =>
      File.WriteAllText(path, JsonSerializer.Serialize(obj, JsonOptions));

    private static T? Deserialize<T>(string path) where T : class =>
      JsonSerializer.Deserialize<T>(File.ReadAllText(path), JsonOptions);
  }
}