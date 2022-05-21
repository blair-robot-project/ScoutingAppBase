using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

#nullable enable

namespace ScoutingAppBase.Data
{
  public sealed class EventData
  {
    public EventData(EventConfig config, List<MatchData> matches)
    {
      Config = config;
      Matches = matches;
    }

    public readonly EventConfig Config;

    public readonly List<MatchData> Matches;
  }

  public sealed class MatchData
  {
    /// <summary>
    /// Whether this match has been sent over to the server
    /// </summary>
    public bool Synced { get; set; } = false;

    public readonly Dictionary<string, object> Fields = new Dictionary<string, object>();

    [JsonConstructor]
    public MatchData(Dictionary<string, object> fields, bool synced)
      => (Fields, Synced) = (fields, synced);

    public object this[FieldConfig fieldConfig] => Fields[fieldConfig.Name];
  }

  public enum Alliance
  {
    Blue, Red
  }
}
