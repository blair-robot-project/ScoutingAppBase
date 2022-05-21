using System.Collections.Generic;
using Xamarin.Forms;

using ScoutingAppBase.Data.Bluetooth;

[assembly: Dependency(typeof(ScoutingAppBase.iOS.Data.Bluetooth.IosGattPeripheralCreator))]
namespace ScoutingAppBase.iOS.Data.Bluetooth
{
  internal class IosGattPeripheralCreator : IGattPeripheralCreator
  {
    public IGattPeripheral Create(List<GattService> services) => new IosGattPeripheral(services);
  }
}