using System.Collections.Generic;
using Xamarin.Forms;

namespace ScoutingAppBase.Bluetooth
{
  /// <summary>
  /// Subclasses will create a platform-specific object representing
  /// a peripheral device
  /// </summary>
  public interface IGattPeripheralManager
  {
    /// <summary>
    /// Create a peripheral device
    /// </summary>
    /// <returns>A platform-specific peripheral device object</returns>
    public GattPeripheral Create(
      List<GattService> services,
      GattPeripheralCallbacks callbacks);

    public void StartAdvertising(GattAdOptions adData);

    public void StopAdvertising();

    /// <summary>
    /// Gets the platform-specific <see cref="IGattPeripheralManager"/>
    /// </summary>
    /// <returns></returns>
    public static IGattPeripheralManager Get()
    {
      return DependencyService.Get<IGattPeripheralManager>();
    }
  }
}