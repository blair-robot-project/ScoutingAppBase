using System.Collections.Generic;

namespace ScoutingAppBase.Bluetooth
{
  /// <summary>
  /// Settings and data for advertising
  /// </summary>
  public sealed class GattAdOptions
  {
    public bool IncludeDeviceName { get; set; } = false;

    public TxPowerLevel? PowerLevel { get; set; }

    /// <summary>
    /// Timeout for advertising, in milliseconds
    /// </summary>
    public int Timeout { get; set; } = 0;

    public List<string> ServiceUuids { get; set; } = new List<string>();

    public List<(int ManufacturerId, byte[] Data)>? ManufacturerSpecificData { get; set; }

    public List<(string Uuid, byte[] Data)>? ServiceData { get; set; }

    // todo add more advertisement data

    public enum TxPowerLevel : uint
    {
      PowerUltraLow = 0,
      PowerLow = 1,
      PowerMedium = 2,
      PowerHigh = 3
    }
  }
}
