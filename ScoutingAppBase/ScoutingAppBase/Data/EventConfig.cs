using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable disable

namespace ScoutingAppBase.Data
{
  public sealed class EventConfig
  {
    [JsonPropertyName("event")]
    public string EventName;

    public int OurTeam { get; set; }

    /// <summary>
    /// Get the configs for the fields specific to this year
    /// </summary>
    [JsonPropertyName("fields")]
    public List<FieldConfig> SpecFieldConfigs { get; set; } = new List<FieldConfig>();

    /// <summary>
    /// Get all field configs, including general ones not specific to this year
    /// </summary>
    [JsonIgnore]
    public List<FieldConfig> AllFieldConfigs => SpecFieldConfigs.Concat(GeneralFields.All).ToList();
  }

  public sealed class FieldConfig
  {
    public string Name { get; set; }

    public FieldType Type { get; set; }

    /// <summary>
    /// The UUID of the corresponding GATT characteristic
    /// </summary>
    public string Uuid { get; set; }

    /// <summary>
    /// Minimum value possible (only for numbers)
    /// </summary>
    public double Min { get; set; } = 0.0;

    /// <summary>
    /// Minimum value possible (only for numbers)
    /// </summary>
    public double Max { get; set; } = 1000.0;

    /// <summary>
    /// Increment for the value (only for numbers)
    /// </summary>
    public double Inc { get; set; } = 1.0;

    /// <summary>
    /// The choices for this field (only for multiple choice)
    /// </summary>
    public List<string> Choices { get; set; } = new List<string>();

    /// <summary>
    /// The choice selected by default (only for multiple choice)
    /// </summary>
    public string DefaultChoice { get; set; }

    public override bool Equals(object obj)
     => (obj is FieldConfig cfg) && Uuid == cfg.Uuid;

    public override int GetHashCode() => Uuid.GetHashCode();
  }

  /// <summary>
  /// Fields that are kept every year
  /// </summary>
  public static class GeneralFields
  {
    public static readonly FieldConfig MatchNum = new FieldConfig
    {
      Name = "matchNum",
      Type = FieldType.Num,
      Uuid = "a3c1723b-b439-43f0-a2b0-acb724c64528",
      Min = 1
    };

    public static readonly FieldConfig Synced = new FieldConfig
    {
      Name = "synced",
      Type = FieldType.Bool,
      Uuid = "1a90e4f9-0310-4ce8-9d18-d460e9661f5b"
    };

    public static readonly FieldConfig RecorderName = new FieldConfig
    {
      Name = "recorderName",
      Type = FieldType.Text,
      Uuid = "63b525ee-5c90-4775-b9d8-2b2de19d43c3"
    };

    public static readonly FieldConfig TeamNum = new FieldConfig
    {
      Name = "teamNum",
      Type = FieldType.Num,
      Uuid = "dce8aaf2-12e3-43c2-a915-fdf6750981fa",
      Min = 1,
      Max = 100_000
    };

    public static readonly FieldConfig Alliance = new FieldConfig
    {
      Name = "alliance",
      Type = FieldType.Choice,
      Uuid = "aa0c3c5e-4f0f-46d7-828c-ec7f7518f41e",
      Choices = { "Blue", "Red" },
      DefaultChoice = "Blue"
    };

    /// <summary>
    /// Which driver station (1, 2, or 3) the team is at
    /// </summary>
    public static readonly FieldConfig Station = new FieldConfig
    {
      Name = "station",
      Type = FieldType.Num,
      Uuid = "3474f023-5a13-4b90-bb01-89eab3a8d58e",
      Min = 1,
      Max = 3
    };

    public static readonly FieldConfig Timestamp = new FieldConfig
    {
      Name = "timestamp",
      Type = FieldType.Text,
      Uuid = "e6343506-5225-44bf-853f-ffe51b20985e"
    };

    public static readonly FieldConfig Comments = new FieldConfig
    {
      Name = "comments",
      Type = FieldType.Text,
      Uuid = "6592806f-553e-47f6-bde4-6d29555227c8"
    };

    /// <summary>
    /// All the <see cref="GeneralFields"/>
    /// </summary>
    public static readonly List<FieldConfig> All = new List<FieldConfig>
    {
      MatchNum, TeamNum, Comments
    };
  }

  public enum FieldType
  {
    Num, Bool, Choice, Text
  }
}
