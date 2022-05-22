using CoreBluetooth;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;

using ScoutingAppBase.Bluetooth;

[assembly: Dependency(typeof(ScoutingAppBase.iOS.Bluetooth.IosGattPeripheralManager))]
namespace ScoutingAppBase.iOS.Bluetooth
{
  internal class IosGattPeripheralManager : GattPeripheralManager
  {

    private readonly CBPeripheralManager Manager;

    public IosGattPeripheralManager()
    {
      Manager = new CBPeripheralManager();
    }

    public GattPeripheral Create(
      List<GattService> services,
      GattPeripheralCallbacks callbacks) => new IosGattPeripheral(Manager, services, callbacks);

    public void StartAdvertising(GattAdOptions adData)
    {
      var iosAdData = new Dictionary<NSString, NSObject>();

      if (adData.IncludeDeviceName)
      {
        iosAdData.Add(CBAdvertisement.DataLocalNameKey, NSObject.FromObject(UIDevice.CurrentDevice.Name));
      }

      if (adData.PowerLevel != null)
      {
        iosAdData.Add(CBAdvertisement.DataTxPowerLevelKey, NSObject.FromObject(adData.PowerLevel));
      }

      if (adData.ServiceUuids != null)
      {
        iosAdData.Add(
          CBAdvertisement.DataServiceUUIDsKey,
          NSArray.FromNSObjects(
            adData.ServiceUuids.Select(CBUUID.FromString).ToArray()
          )
        );
      }

      if (adData.ManufacturerSpecificData != null)
      {
        // todo figure out how to send this
        iosAdData.Add(CBAdvertisement.DataManufacturerDataKey, null);
      }

      if (adData.ServiceData != null)
      {
        iosAdData.Add(
          CBAdvertisement.DataServiceDataKey,
          NSDictionary.FromObjectsAndKeys(
            adData.ServiceData.Select(serviceData => CBUUID.FromString(serviceData.Uuid)).ToArray(),
            adData.ServiceData.Select(serviceData => NSData.FromArray(serviceData.Data)).ToArray()
          )
        );
      }

      Manager.StartAdvertising(
        NSDictionary.FromObjectsAndKeys(
          iosAdData.Keys.ToArray(), iosAdData.Values.ToArray()));
    }

    public void StopAdvertising()
    {
      Manager.StopAdvertising();
    }
  }
}