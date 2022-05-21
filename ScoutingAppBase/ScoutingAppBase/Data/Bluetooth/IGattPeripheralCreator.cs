using System.Collections.Generic;
using Xamarin.Forms;

namespace ScoutingAppBase.Data.Bluetooth
{
  /// <summary>
  /// Subclasses will create a platform-specific object representing
  /// a peripheral device
  /// </summary>
  public interface IGattPeripheralCreator
  {
    /// <summary>
    /// Create a peripheral device
    /// </summary>
    /// <returns>A platform-specific peripheral device object</returns>
    public abstract IGattPeripheral Create(List<GattService> services);

    /// <summary>
    /// Gets the platform-specific <see cref="IGattPeripheralCreator"/>
    /// </summary>
    /// <returns></returns>
    public static IGattPeripheralCreator Get()
    {
      return DependencyService.Get<IGattPeripheralCreator>();
    }
  }
}
