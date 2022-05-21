using CoreBluetooth;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

using ScoutingAppBase.Data;
using ScoutingAppBase.Data.Bluetooth;

namespace ScoutingAppBase.iOS.Data.Bluetooth
{
  internal class IosGattPeripheral : IGattPeripheral
  {
    public IosGattPeripheral(List<GattService> services)
    {
      var manager = new CBPeripheralManager();

      // Add our custom service and characteristics
      var serviceUuid = CBUUID.FromString(config.ServiceUuid);
      var service = new CBMutableService(serviceUuid, true);

      service.Characteristics = Config.FieldConfigs.Select(fieldConfig =>
          new CBMutableCharacteristic(
            CBUUID.FromString(fieldConfig.CharUuid),
            CBCharacteristicProperties.Read
            | CBCharacteristicProperties.Write
            | CBCharacteristicProperties.Notify,
            null,
            CBAttributePermissions.Readable
            | CBAttributePermissions.Writeable
          )
        ).ToArray();

      manager.AddService(service);

      var adData = NSDictionary.FromObjectAndKey(
        CBAdvertisement.DataLocalNameKey,
        NSArray.FromNSObjects(serviceUuid)
      );
      manager.StartAdvertising(adData);
    }

    public void Dispose()
    {
      // todo implement
    }

    public class PeripheralDelegateImpl : CBPeripheralManagerDelegate
    {
      public override void StateUpdated(CBPeripheralManager peripheral)
      {
        throw new NotImplementedException();
      }
    }
  }
}
