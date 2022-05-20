using Xamarin.Forms;

using ScoutingAppBase.Data.Bluetooth;
using ScoutingAppBase.Data;

[assembly: Dependency(typeof(ScoutingAppBase.iOS.Data.Bluetooth.IosGattPeripheralCreator))]
namespace ScoutingAppBase.iOS.Data.Bluetooth
{
  internal class IosGattPeripheralCreator : IGattPeripheralCreator
  {
    public IGattPeripheral Create(EventConfig config)
    {
      return new IosGattPeripheral(config);
    }
  }
}