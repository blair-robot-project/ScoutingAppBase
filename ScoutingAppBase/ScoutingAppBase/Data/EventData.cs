using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable

namespace ScoutingAppBase.Data
{
  public sealed class EventData
  {
    
    public readonly EventConfig Config;

    public readonly List<MatchData> Matches;

    public EventData(EventConfig config, IEnumerable<MatchData> matches)
    {
      Config = config;
      Matches = matches.ToList();
    }
  }

  public sealed class MatchData
  {
    public int MatchNum => (int) this[GeneralFields.MatchNum];
    
    /// <summary>
    /// Whether this match has been sent over to the server
    /// </summary>
    public bool Synced
    {
      get => (bool) this[GeneralFields.Synced];
      set => this[GeneralFields.Synced] = value;
    }

    public readonly Dictionary<string, object> Fields;

    public MatchData(int matchNum, bool synced)
    {
      Fields = new Dictionary<string, object>
      {
        [GeneralFields.MatchNum.Name] = matchNum,
        [GeneralFields.Synced.Name] = synced
      };
    }

    [JsonConstructor]
    public MatchData(Dictionary<string, object> fields) => Fields = fields;

    public object this[FieldConfig fieldConfig]
    {
      get => Fields[fieldConfig.Name];
      set => Fields[fieldConfig.Name] = value;
    }
  }
}
