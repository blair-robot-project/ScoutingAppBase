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
    /// The UUID of the custom service
    /// </summary>
    public string ServiceUuid { get; set; }

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
    public List<string> Choices { get; set; }

    /// <summary>
    /// The choice selected by default (only for multiple choice)
    /// </summary>
    public string DefaultChoice { get; set; }
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
      Min = 1
    };

    public static readonly FieldConfig Synced = new FieldConfig
    {
      Name = "synced",
      Type = FieldType.Bool
    };

    public static readonly FieldConfig TeamNum = new FieldConfig
    {
      Name = "teamNum",
      Type = FieldType.Num,
      Min = 1,
      Max = 100_000
    };

    public static readonly FieldConfig Comments = new FieldConfig
    {
      Name = "comments",
      Type = FieldType.Text
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
