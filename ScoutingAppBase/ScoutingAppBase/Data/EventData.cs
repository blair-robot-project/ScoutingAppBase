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
    [JsonConstructor]
    public MatchData(string? id, Alliance alliance, int robotNum, bool synced, Dictionary<string, string> fields)
      => (Id, Alliance, RobotNum, Synced, Fields) = (id, alliance, robotNum, synced, fields);

    public MatchData() : this(null, Alliance.Blue, 1, false, new Dictionary<string, string>()) { }

    public string? Id { get; set; }

    /// <summary>
    /// The alliance of the robot to scout
    /// </summary>
    public Alliance Alliance { get; set; }

    /// <summary>
    /// The number of the robot on the alliance, e.g. 1 for R1 (not the team number)
    /// </summary>
    public int RobotNum { get; set; }

    /// <summary>
    /// Whether this match has been sent over to the server
    /// </summary>
    public bool Synced { get; set; } = false;

    public string Comments { get; set; } = false;

    internal readonly Dictionary<string, string> Fields = new Dictionary<string, string>();
  }

  public enum Alliance
  {
    Blue, Red
  }
}
