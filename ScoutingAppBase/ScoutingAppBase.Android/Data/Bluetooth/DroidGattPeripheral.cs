using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.Util;
using ScoutingAppBase.Data.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoutingAppBase.Droid.Data.Bluetooth
{
  internal class DroidGattPeripheral : IGattPeripheral
  {
    private readonly BluetoothManager Manager;
    private readonly BluetoothAdapter Adapter;
    private readonly BluetoothGattServer GattServer;

    public DroidGattPeripheral(List<GattService> services)
    {
      Manager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
      Adapter = Manager.Adapter;

      GattServer = Manager.OpenGattServer(Application.Context, new GattServerCallbackImpl())!;

      // Add our custom service and characteristics
      foreach (var service in services)
      {
        var serviceType = service.ServiceType == GattService.Type.Primary
            ? GattServiceType.Primary
            : GattServiceType.Secondary;
        var droidService = new BluetoothGattService(
          UUID.FromString(service.Uuid),
          serviceType
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

      // Start advertising
      var adSettings = new AdvertiseSettings.Builder()
        .SetConnectable(true)
        .SetTimeout(0)
        .SetAdvertiseMode(AdvertiseMode.LowLatency)
        .SetTxPowerLevel(AdvertiseTx.PowerHigh)
        .Build();

      var adDataBuilder = new AdvertiseData.Builder()
        .SetIncludeDeviceName(true)
        .SetIncludeTxPowerLevel(true);

      foreach (var service in services)
      {
        adDataBuilder.AddServiceUuid(new ParcelUuid(UUID.FromString(service.Uuid)));
      }

      Adapter.BluetoothLeAdvertiser.StartAdvertising(
        adSettings, adDataBuilder.Build(), new AdvertiseCallbackImpl());
    }

    public void Dispose()
    {
      // todo implement
    }

    /// <summary>
    /// Convert one of our Property objects to an Android flag
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private static GattProperty ToDroidProp(GattCharacteristic.Property prop) => prop switch
    {
      GattCharacteristic.Property.Broadcast => GattProperty.Broadcast,
      GattCharacteristic.Property.Read => GattProperty.Read,
      GattCharacteristic.Property.WriteNoResponse => GattProperty.WriteNoResponse,
      GattCharacteristic.Property.Write => GattProperty.Write,
      GattCharacteristic.Property.Notify => GattProperty.Notify,
      GattCharacteristic.Property.Indicate => GattProperty.Indicate,
      GattCharacteristic.Property.SignedWrite => GattProperty.SignedWrite,
      GattCharacteristic.Property.ExtendedProps => GattProperty.ExtendedProps,
      _ => 0 // Unsupported by Android
    };

    /// <summary>
    /// Convert one of our Permission objects to an Android flag
    /// </summary>
    /// <param name="perm"></param>
    /// <returns></returns>
    private static GattPermission ToDroidPerm(GattCharacteristic.Permission perm) => perm switch
    {
      GattCharacteristic.Permission.Read => GattPermission.Read,
      GattCharacteristic.Permission.ReadEncrypted => GattPermission.ReadEncrypted,
      GattCharacteristic.Permission.ReadEncryptedMitm => GattPermission.Read,
      GattCharacteristic.Permission.Write => GattPermission.Read,
      GattCharacteristic.Permission.WriteEncrypted => GattPermission.Read,
      GattCharacteristic.Permission.WriteEncryptedMitm => GattPermission.Read,
      GattCharacteristic.Permission.WriteSigned => GattPermission.Read,
      GattCharacteristic.Permission.WriteSignedMitm => GattPermission.Read,
      _ => throw new ArgumentOutOfRangeException(nameof(perm), $"Unknown permission {perm}")
    };

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