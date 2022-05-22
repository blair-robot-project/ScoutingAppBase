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
    /// <summary>
    /// Maps UUIDs to their iOS characteristic objects
    /// </summary>
    private readonly Dictionary<string, CBMutableCharacteristic> ToIosChar =
      new Dictionary<string, CBMutableCharacteristic>();

    public IosGattPeripheral(
      CBPeripheralManager manager,
      List<GattService> services,
      GattPeripheralCallbacks callbacks)
    {
      SetupCallbacks(manager, callbacks);

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
          var iosChar = new CBMutableCharacteristic(
            CBUUID.FromString(characteristic.Uuid), props, null, perms
          );
          ToIosChar[characteristic.Uuid] = iosChar;
          return iosChar;
        }).ToArray<CBCharacteristic>();

        manager.AddService(iosService);
      }
    }

    public override byte[] ReadCharacteristic(string uuid)
    {
      return ToIosChar[uuid].Value?.ToArray() ?? new byte[] { };
    }

    public override void WriteCharacteristic(string uuid, byte[] value)
    {
      ToIosChar[uuid].Value = NSData.FromArray(value);
    }

    private static void SetupCallbacks(CBPeripheralManager manager, GattPeripheralCallbacks callbacks)
    {
      // todo implement other callbacks
      if (callbacks.OnReadRequest != null)
      {
        manager.ReadRequestReceived +=
          (_, args) => callbacks.OnReadRequest(args.Request.Characteristic.UUID.Uuid);
      }

      if (callbacks.OnWriteRequest != null)
      {
        manager.WriteRequestsReceived += (_, args) =>
        {
          foreach (var request in args.Requests)
          {
            if (request.Value != null)
            {
              callbacks.OnWriteRequest(request.Characteristic.UUID.Uuid, request.Value.ToArray());
            }
          }
        };
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

    public override void Dispose()
    {
      // todo implement
    }
  }
}