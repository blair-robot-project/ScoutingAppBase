using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.Util;
using ScoutingAppBase.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoutingAppBase.Droid.Bluetooth
{
  internal class DroidGattPeripheral : GattPeripheral
  {
    private readonly BluetoothManager Manager;
    private readonly BluetoothAdapter Adapter;
    private readonly BluetoothGattServer GattServer;

    public DroidGattPeripheral(
      BluetoothManager manager,
      List<GattService> services,
      GattPeripheralCallbacks callbacks) : base(services, callbacks)
    {
      Manager = manager;
      Adapter = Manager.Adapter;

      GattServer = Manager.OpenGattServer(Application.Context, new GattServerCallbackImpl())!;

      // Add our custom service and characteristics
      foreach (var service in services)
      {
        var droidService = new BluetoothGattService(
          UUID.FromString(service.Uuid),
          service.IsPrimary ? GattServiceType.Primary : GattServiceType.Secondary
        );

        foreach (var characteristic in service.Characteristics)
        {
          GattProperty props = characteristic.Properties
            .Select(ToDroidProp)
            .Aggregate((p1, p2) => p1 | p2);
          GattPermission perms = characteristic.Permissions
            .Select(ToDroidPerm)
            .Aggregate((p1, p2) => p1 | p2);
          var droidChar = new BluetoothGattCharacteristic(
            UUID.FromString(characteristic.Uuid), props, perms
          );
          droidService.AddCharacteristic(droidChar);
        }

        GattServer.AddService(droidService);
      }
    }

    /// <summary>
    /// Convert one of our Property objects to an Android flag
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private static GattProperty ToDroidProp(GattChar.Property prop)
    {
      switch (prop)
      {
        case GattChar.Property.Broadcast: return GattProperty.Broadcast;
        case GattChar.Property.Read: return GattProperty.Read;
        case GattChar.Property.WriteNoResponse: return GattProperty.WriteNoResponse;
        case GattChar.Property.Write: return GattProperty.Write;
        case GattChar.Property.Notify: return GattProperty.Notify;
        case GattChar.Property.Indicate: return GattProperty.Indicate;
        case GattChar.Property.SignedWrite: return GattProperty.SignedWrite;
        case GattChar.Property.ExtendProps: return GattProperty.ExtendedProps;
        default: throw new ArgumentOutOfRangeException(nameof(prop), $"Unknown property {prop}");
      }
    }

    /// <summary>
    /// Convert one of our Permission objects to an Android flag
    /// </summary>
    /// <param name="perm"></param>
    /// <returns></returns>
    private static GattPermission ToDroidPerm(GattChar.Permission perm)
    {
      switch (perm)
      {
        case GattChar.Permission.Read: return GattPermission.Read;
        case GattChar.Permission.ReadEncrypted: return GattPermission.ReadEncrypted;
        case GattChar.Permission.ReadEncryptedMitm: return GattPermission.ReadEncryptedMitm;
        case GattChar.Permission.Write: return GattPermission.Write;
        case GattChar.Permission.WriteEncrypted: return GattPermission.WriteEncrypted;
        case GattChar.Permission.WriteEncryptedMitm: return GattPermission.WriteEncryptedMitm;
        case GattChar.Permission.WriteSigned: return GattPermission.WriteSigned;
        case GattChar.Permission.WriteSignedMitm: return GattPermission.WriteSignedMitm;
        default: throw new ArgumentOutOfRangeException(nameof(perm), $"Unknown permission {perm}");
      }
    }


    override public void Dispose()
    {
      // todo implement
    }

    private class GattServerCallbackImpl : BluetoothGattServerCallback
    {
      public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic)
      {
        base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);
      }

      public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
      {
        base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);
      }

      public override void OnConnectionStateChange(BluetoothDevice device, [GeneratedEnum] ProfileState status, [GeneratedEnum] ProfileState newState)
      {
        base.OnConnectionStateChange(device, status, newState);
        if (status == ProfileState.Connected)
        {

        }
      }

      public override void OnNotificationSent(BluetoothDevice device, [GeneratedEnum] GattStatus status)
      {
        base.OnNotificationSent(device, status);
      }
    }

    private class AdvertiseCallbackImpl : AdvertiseCallback
    {
      public override void OnStartFailure(AdvertiseFailure errorCode)
      {
        base.OnStartFailure(errorCode);
      }
    }
  }
}