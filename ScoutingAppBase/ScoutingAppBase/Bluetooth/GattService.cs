using System.Collections.Generic;
using System.Linq;

namespace ScoutingAppBase.Bluetooth
{
  /// <summary>
  /// Settings for a custom GATT service
  /// </summary>
  public class GattService
  {
    public readonly string Uuid;

    /// <summary>
    /// Whether this is the primary service for the device
    /// </summary>
    public readonly bool IsPrimary;

    public readonly ISet<GattChar> Characteristics;

    public GattService(string uuid, bool isPrimary, IEnumerable<GattChar> characteristics)
    {
      (Uuid, IsPrimary, Characteristics) = (uuid, isPrimary, characteristics.ToHashSet());
    }
  }
}
