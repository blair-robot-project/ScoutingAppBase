using System.Collections.Generic;
using Xamarin.Forms;

using ScoutingAppBase.Data.Bluetooth;
using ScoutingAppBase.Data;

[assembly: Dependency(typeof(ScoutingAppBase.Droid.Data.Bluetooth.DroidGattPeripheralCreator))]
namespace ScoutingAppBase.Droid.Data.Bluetooth
{
  internal class DroidGattPeripheralCreator : IGattPeripheralCreator
  {
    public IGattPeripheral Create(EventConfig config)
    {
      return new DroidGattPeripheral(config);
    }
  }
}
