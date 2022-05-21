using ScoutingAppBase.Bluetooth;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ScoutingAppBase.Bluetooth
{
  /// <summary>
  /// Subclasses will create a platform-specific object representing
  /// a peripheral device
  /// </summary>
  public interface GattPeripheralManager
  {
    /// <summary>
    /// Create a peripheral device
    /// </summary>
    /// <returns>A platform-specific peripheral device object</returns>
    public abstract GattPeripheral Create(
      List<GattService> services,
      GattPeripheralCallbacks callbacks);

    /// <summary>
    /// Gets the platform-specific <see cref="GattPeripheralManager"/>
    /// </summary>
    /// <returns></returns>
    public static GattPeripheralManager Get()
    {
      return DependencyService.Get<GattPeripheralManager>();
    }

    public abstract void StartAdvertising(GattAdOptions adData);

    public abstract void StopAdvertising();
  }
}
