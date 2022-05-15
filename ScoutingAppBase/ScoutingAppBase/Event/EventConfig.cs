using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ScoutingAppBase.Event
{
  public class EventConfig
  {
    public string EventName;

    public int TeamNumber { get; set; }

    public List<FieldConfig> FieldConfigs;
  }

  public class FieldConfig
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
    /// Whether it's an integer (only for numbers)
    /// </summary>
    public bool IsInt { get; set; }

    /// <summary>
    /// The choices for this field (only for radio button groups)
    /// </summary>
    public List<string> Choices { get; set; }
  }

  public enum FieldType
  {
    Num, Bool, Radio, Text
  }
}
