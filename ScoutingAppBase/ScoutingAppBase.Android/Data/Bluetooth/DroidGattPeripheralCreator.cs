using System.Collections.Generic;
using Xamarin.Forms;

using ScoutingAppBase.Data.Bluetooth;

[assembly: Dependency(typeof(ScoutingAppBase.Droid.Data.Bluetooth.DroidGattPeripheralCreator))]
namespace ScoutingAppBase.Droid.Data.Bluetooth
{
  internal class DroidGattPeripheralCreator : IGattPeripheralCreator
  {
    public IGattPeripheral Create(List<GattService> services) => new DroidGattPeripheral(services);
  }
}
