using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ScoutingAppBase.Bluetooth;

namespace ScoutingAppBase.Data
{
  public class DataManager
  {
    private readonly GattPeripheralManager Manager;

    private readonly GattPeripheral Peripheral;

    /// <summary>
    /// Keys are field names, values are corresponding characteristics
    /// </summary>
    private readonly Dictionary<string, GattChar> FieldsToChars;

    /// <summary>
    /// Keys are characteristic UUIDs, values are corresponding field configs
    /// </summary>
    private readonly Dictionary<string, FieldConfig> CharsToFields;

    /// <summary>
    /// The match that's currently being synced
    /// </summary>
    private MatchData? CurrMatch;

    /// <summary>
    /// Matches to send
    /// </summary>
    private readonly ConcurrentQueue<MatchData> Matches = new ConcurrentQueue<MatchData>();

    public DataManager(EventConfig config)
    {
      Manager = GattPeripheralManager.Get();

      FieldsToChars = new Dictionary<string, GattChar>();
      CharsToFields = new Dictionary<string, FieldConfig>();
      foreach (var field in config.AllFieldConfigs)
      {
        var gattChar = new GattChar(
          field.Uuid,
          new List<GattChar.Property>
          { GattChar.Property.Read, GattChar.Property.Write, GattChar.Property.Notify },
          new List<GattChar.Permission>
          { GattChar.Permission.Read, GattChar.Permission.Write }
        );
        FieldsToChars.Add(field.Name, gattChar);
        CharsToFields.Add(field.Uuid, field);
      }

      var service = new GattService(config.ServiceUuid, true, FieldsToChars.Values);

      Peripheral = Manager.Create(
          new List<GattService> { service },
          new GattPeripheralCallbacks
          {
            OnWriteRequest = OnWriteRequest
          }
        );

      Manager.StartAdvertising(new GattAdOptions
      {
        IncludeDeviceName = true,
        PowerLevel = GattAdOptions.TxPowerLevel.PowerHigh,
        ServiceUuids = { service.Uuid }
      });
    }

    public void SendMatch(MatchData match) => Matches.Enqueue(match);

    private void OnWriteRequest(string uuid, byte[] value)
    {
      var fieldConfig = CharsToFields[uuid];
      if (uuid == GeneralFields.Synced.Uuid)
      {
        Debug.Assert(CurrMatch != null);
        var synced = (bool)Decode(fieldConfig, value);
        CurrMatch!.Synced = synced;
        // If it didn't sync, process this match again later
        if (!synced)
          Matches.Enqueue(CurrMatch!);
      }

      // If there are more matches, start on the next one
      if (!Matches.IsEmpty)
      {
        var successful = Matches.TryDequeue(out CurrMatch);
        Debug.Assert(successful);
        if (successful)
        {
          WriteMatch(CurrMatch);
        }
        // todo handle dequeue failing
      }
    }

    /// <summary>
    /// Write to the characteristics corresponding to this match's fields
    /// </summary>
    /// <param name="match"></param>
    private void WriteMatch(MatchData match)
    {
      foreach (var (fieldName, field) in match.Fields)
      {
        var encoded = Encode(FieldsToChars[fieldName], field);
      }
    }

    /// <summary>
    /// Encode a field as a byte array
    /// </summary>
    private byte[] Encode(FieldConfig fieldConfig, object field)
    {
      return fieldConfig.Type switch
      {
        FieldType.Num => BitConverter.GetBytes((double)field),
        FieldType.Bool => BitConverter.GetBytes((bool)field),
        FieldType.Text => Encoding.ASCII.GetBytes((string)field),
        FieldType.Choice => Encoding.ASCII.GetBytes((string)field),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    /// <summary>
    /// Decode a characteristic value given the corresponding field config
    /// </summary>
    /// <param name="value">The value of the characteristic</param>
    /// <returns></returns>
    private object Decode(FieldConfig fieldConfig, byte[] value)
    {
      return fieldConfig.Type switch
      {
        FieldType.Num => BitConverter.ToDouble(value),
        FieldType.Bool => BitConverter.ToBoolean(value),
        FieldType.Text => Encoding.ASCII.GetString(value),
        FieldType.Choice => Encoding.ASCII.GetString(value),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}
