using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#nullable enable

namespace ScoutingAppBase.Event
{

  public class Event
  {
    public Event(EventConfig config, List<Match> matches)
    {
      Config = config;
      Matches = matches;
    }

    public readonly EventConfig Config;

    public readonly List<Match> Matches;
  }

  public class Match
  {
    public Match(List<FieldConfig> fieldConfigs)
    {
      FieldConfigs = fieldConfigs.ToDictionary(fieldConfig => fieldConfig.Name);
    }

    /// <summary>
    /// Whether this match has been sent over to the server
    /// </summary>
    public bool Synced { get; set; } = false;

    public string? MatchName { get; set; }

    /// <summary>
    /// Whether this match is a replay
    /// </summary>
    public bool IsReplay { get; set; } = false;

    public readonly Dictionary<string, FieldConfig> FieldConfigs;

    /// <summary>
    /// The alliance of the robot to scout
    /// </summary>
    public Alliance Alliance { get; set; }

    /// <summary>
    /// The number of the robot on the alliance, e.g. 1 for R1 (not the team number)
    /// </summary>
    public int RobotNum { get; set; }

    private readonly Dictionary<string, string> Fields = new Dictionary<string, string>();

    public void SetNum(string key, double value)
    {
      Debug.Assert(FieldConfigs.ContainsKey(key));

      var cfg = FieldConfigs[key];
      Debug.Assert(cfg.Type == FieldType.Num);
      Debug.Assert(value >= cfg.Min && value <= cfg.Max);

      Fields.Add(key, value.ToString());
    }

    public void SetBool(string key, bool value)
    {
      Debug.Assert(FieldConfigs.ContainsKey(key));
      Debug.Assert(FieldConfigs[key].Type == FieldType.Bool);

      Fields.Add(key, value.ToString());
    }

    public void SetRadio(string key, int choice)
    {
      Debug.Assert(FieldConfigs.ContainsKey(key));

      var cfg = FieldConfigs[key];
      Debug.Assert(cfg.Type == FieldType.Radio);
      Debug.Assert(choice >= 0 && choice < cfg.Choices.Count());

      Fields.Add(key, cfg.Choices[choice]);
    }
  }

  public enum Alliance
  {
    Blue, Red
  }
}
