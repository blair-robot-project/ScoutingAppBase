using CoreBluetooth;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

using ScoutingAppBase.Data;
using ScoutingAppBase.Bluetooth;

namespace ScoutingAppBase.iOS.Bluetooth
{
  internal class IosGattPeripheral : GattPeripheral
  {
    private readonly CBPeripheralManager Manager;

    public IosGattPeripheral(
      CBPeripheralManager manager,
      List<GattService> services,
      GattPeripheralCallbacks callbacks) : base(services, callbacks)
    {
      Manager = manager;

      // Add our custom service and characteristics
      foreach (var service in services)
      {
        var iosService = new CBMutableService(
          CBUUID.FromString(service.Uuid),
          service.IsPrimary
        );

        iosService.Characteristics = service.Characteristics.Select(characteristic =>
        {
          CBCharacteristicProperties props = characteristic.Properties
            .Select(ToIosProp)
            .Aggregate((p1, p2) => p1 | p2);
          CBAttributePermissions perms = characteristic.Permissions
            .Select(ToIosPerm)
            .Aggregate((p1, p2) => p1 | p2);
          return new CBMutableCharacteristic(
            CBUUID.FromString(characteristic.Uuid), props, null, perms
          );
        }).ToArray();

        Manager.AddService(iosService);
      }
    }

    /// <summary>
    /// Convert one of our Property objects to an Android flag
    /// </summary>
    private static CBCharacteristicProperties ToIosProp(GattChar.Property prop)
    {
      switch (prop)
      {
        case GattChar.Property.Broadcast: return CBCharacteristicProperties.Broadcast;
        case GattChar.Property.Read: return CBCharacteristicProperties.Read;
        case GattChar.Property.WriteNoResponse: return CBCharacteristicProperties.WriteWithoutResponse;
        case GattChar.Property.Write: return CBCharacteristicProperties.Write;
        case GattChar.Property.Notify: return CBCharacteristicProperties.Notify;
        case GattChar.Property.Indicate: return CBCharacteristicProperties.Indicate;
        case GattChar.Property.SignedWrite: return CBCharacteristicProperties.AuthenticatedSignedWrites;
        case GattChar.Property.ExtendProps: return CBCharacteristicProperties.ExtendedProperties;
        default: throw new ArgumentOutOfRangeException(nameof(prop), $"Unknown property {prop}");
      }
    }

    /// <summary>
    /// Convert one of our Permission objects to an Android flag
    /// </summary>
    /// <param name="perm"></param>
    /// <returns></returns>
    private static CBAttributePermissions ToIosPerm(GattChar.Permission perm)
    {
      switch (perm)
      {
        case GattChar.Permission.Read: return CBAttributePermissions.Readable;
        case GattChar.Permission.ReadEncrypted: return CBAttributePermissions.ReadEncryptionRequired;
        case GattChar.Permission.ReadEncryptedMitm: return CBAttributePermissions.ReadEncryptionRequired;
        case GattChar.Permission.Write: return CBAttributePermissions.Writeable;
        case GattChar.Permission.WriteEncrypted: return CBAttributePermissions.WriteEncryptionRequired;
        case GattChar.Permission.WriteEncryptedMitm: return CBAttributePermissions.WriteEncryptionRequired;
        case GattChar.Permission.WriteSigned: return CBAttributePermissions.Writeable;
        case GattChar.Permission.WriteSignedMitm: return CBAttributePermissions.Writeable;
        default: throw new ArgumentOutOfRangeException(nameof(perm), $"Unknown permission {perm}");
      }
    }

    override public void Dispose()
    {
      // todo implement
    }

    public class PeripheralDelegateImpl : CBPeripheralManagerDelegate
    {
      private readonly GattPeripheralCallbacks Callbacks;

      public PeripheralDelegateImpl(GattPeripheralCallbacks callbacks) =>
        Callbacks = callbacks;

      public override void StateUpdated(CBPeripheralManager peripheral)
      {
        
        throw new NotImplementedException();
      }

      public override void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
      {
        base.ReadRequestReceived(peripheral, request);
      }
    }
  }
}
