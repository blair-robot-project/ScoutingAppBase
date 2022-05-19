using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace ScoutingAppBase.Data
{
  public sealed class EventConfig
  {
    [JsonPropertyName("event")]
    public string EventName;

    public int OurTeam { get; set; }

    [JsonPropertyName("fields")]
    public List<FieldConfig> FieldConfigs = new List<FieldConfig>();
  }

  public sealed class FieldConfig
  {
    public string Name { get; set; }

    public FieldType Type { get; set; }

    /// <summary>
    /// Minimum value possible (only for numbers)
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Minimum value possible (only for numbers)
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Increment for the value (only for numbers)
    /// </summary>
    public double Inc { get; set; }

    /// <summary>
    /// The choices for this field (only for radio groups)
    /// </summary>
    public List<string> Choices { get; set; }

    /// <summary>
    /// The choice selected by default (only for radio groups)
    /// </summary>
    public string DefaultChoice { get; set; }
  }

  public enum FieldType
  {
    Num, Bool, Radio, Text
  }
}
