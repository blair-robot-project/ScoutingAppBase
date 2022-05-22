using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ScoutingAppBase.Bluetooth;

namespace ScoutingAppBase.Data
{
  public class DataManager
  {
    private readonly GattPeripheral Peripheral;

    private readonly Action<MatchData> OnSync;

    /// <summary>
    /// Keys are field names, values are corresponding characteristic UUIDs
    /// </summary>
    private readonly Dictionary<string, string> FieldsToChars;

    /// <summary>
    /// Keys are field names, values are the corresponding field config
    /// </summary>
    private readonly Dictionary<string, FieldConfig> FieldNamesToConfigs;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="onSync">Action to execute when a match is successfully synced</param>
    public DataManager(EventConfig config, Action<MatchData> onSync)
    {
      OnSync = onSync;

      IGattPeripheralManager manager = IGattPeripheralManager.Get();

      FieldsToChars = new Dictionary<string, string>();
      FieldNamesToConfigs = new Dictionary<string, FieldConfig>();
      CharsToFields = new Dictionary<string, FieldConfig>();
      var chars = new List<GattChar>();
      foreach (var field in config.AllFieldConfigs)
      {
        var gattChar = new GattChar(
          field.Uuid,
          new List<GattChar.Property>
            {GattChar.Property.Read, GattChar.Property.Write, GattChar.Property.Notify},
          new List<GattChar.Permission>
            {GattChar.Permission.Read, GattChar.Permission.Write}
        );
        FieldsToChars.Add(field.Name, field.Uuid);
        FieldNamesToConfigs.Add(field.Name, field);
        CharsToFields.Add(field.Uuid, field);
        chars.Add(gattChar);
      }

      var service = new GattService(config.ServiceUuid, true, chars);

      Peripheral = manager.Create(
        new List<GattService> {service},
        new GattPeripheralCallbacks
        {
          OnWriteRequest = OnWriteRequest
        }
      );

      manager.StartAdvertising(
        new GattAdOptions
        {
          IncludeDeviceName = true,
          PowerLevel = GattAdOptions.TxPowerLevel.PowerHigh,
          ServiceUuids = {service.Uuid}
        }
      );
    }

    public void SendMatch(MatchData match)
    {
      if (Matches.IsEmpty)
      {
        // If there's no other matches to sync, immediately start syncing this
        CurrMatch = match;
        WriteMatch(match);
      } else
      {
        Matches.Enqueue(match);
      }
    }

    private void OnWriteRequest(string uuid, byte[] value)
    {
      var fieldConfig = CharsToFields[uuid];
      if (uuid == GeneralFields.Synced.Uuid)
      {
        Debug.Assert(CurrMatch != null);
        var synced = (bool) Decode(fieldConfig, value);
        CurrMatch!.Synced = synced;
        if (synced)
        {
          OnSync(CurrMatch);
        } else
        {
          // If it didn't sync, process this match again later
          Matches.Enqueue(CurrMatch!);
        }
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
        Peripheral.WriteCharacteristic(FieldsToChars[fieldName], Encode(FieldNamesToConfigs[fieldName], field));
      }
    }

    /// <summary>
    /// Encode a field as a byte array
    /// </summary>
    private byte[] Encode(FieldConfig fieldConfig, object field)
    {
      return fieldConfig.Type switch
      {
        FieldType.Num => BitConverter.GetBytes((double) field),
        FieldType.Bool => BitConverter.GetBytes((bool) field),
        FieldType.Text => Encoding.ASCII.GetBytes((string) field),
        FieldType.Choice => Encoding.ASCII.GetBytes((string) field),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    /// <summary>
    /// Decode a characteristic value given the corresponding field config
    /// </summary>
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