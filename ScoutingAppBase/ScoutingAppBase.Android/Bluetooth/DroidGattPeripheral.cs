using Android.App;
using Android.Bluetooth;
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
    /// <summary>
    /// Maps UUIDs to their Android characteristic objects
    /// </summary>
    private readonly Dictionary<string, BluetoothGattCharacteristic> ToDroidChar =
      new Dictionary<string, BluetoothGattCharacteristic>();

    public DroidGattPeripheral(
      BluetoothManager manager,
      List<GattService> services,
      GattPeripheralCallbacks callbacks)
    {
      BluetoothGattServer gattServer =
        manager.OpenGattServer(Application.Context, new GattServerCallbackImpl(callbacks))!;

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
          ToDroidChar[characteristic.Uuid] = droidChar;
          droidService.AddCharacteristic(droidChar);
        }

        gattServer.AddService(droidService);
      }
    }

    public override byte[] ReadCharacteristic(string uuid)
    {
      return ToDroidChar[uuid].GetValue();
    }

    public override void WriteCharacteristic(string uuid, byte[] value)
    {
      ToDroidChar[uuid].SetValue(value);
    }

    /// <summary>
    /// Convert one of our Property objects to an Android flag
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private static GattProperty ToDroidProp(GattChar.Property prop)
    {
      return prop switch
      {
        GattChar.Property.Broadcast => GattProperty.Broadcast,
        GattChar.Property.Read => GattProperty.Read,
        GattChar.Property.WriteNoResponse => GattProperty.WriteNoResponse,
        GattChar.Property.Write => GattProperty.Write,
        GattChar.Property.Notify => GattProperty.Notify,
        GattChar.Property.Indicate => GattProperty.Indicate,
        GattChar.Property.SignedWrite => GattProperty.SignedWrite,
        GattChar.Property.ExtendProps => GattProperty.ExtendedProps,
        _ => throw new ArgumentOutOfRangeException(nameof(prop), $"Unknown property {prop}")
      };
    }

    /// <summary>
    /// Convert one of our Permission objects to an Android flag
    /// </summary>
    /// <param name="perm"></param>
    /// <returns></returns>
    private static GattPermission ToDroidPerm(GattChar.Permission perm)
    {
      return perm switch
      {
        GattChar.Permission.Read => GattPermission.Read,
        GattChar.Permission.ReadEncrypted => GattPermission.ReadEncrypted,
        GattChar.Permission.ReadEncryptedMitm => GattPermission.ReadEncryptedMitm,
        GattChar.Permission.Write => GattPermission.Write,
        GattChar.Permission.WriteEncrypted => GattPermission.WriteEncrypted,
        GattChar.Permission.WriteEncryptedMitm => GattPermission.WriteEncryptedMitm,
        GattChar.Permission.WriteSigned => GattPermission.WriteSigned,
        GattChar.Permission.WriteSignedMitm => GattPermission.WriteSignedMitm,
        _ => throw new ArgumentOutOfRangeException(nameof(perm), $"Unknown permission {perm}")
      };
    }


    public override void Dispose()
    {
      // todo implement
    }

    private class GattServerCallbackImpl : BluetoothGattServerCallback
    {
      private readonly GattPeripheralCallbacks Callbacks;

      public GattServerCallbackImpl(GattPeripheralCallbacks callbacks) => Callbacks = callbacks;

      public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
        BluetoothGattCharacteristic characteristic)
      {
        base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);
        if (characteristic.Uuid != null)
        {
          Callbacks.OnReadRequest?.Invoke(characteristic.Uuid.ToString());
        }
      }

      public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId,
        BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
      {
        base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset,
          value);
        if (characteristic.Uuid != null)
        {
          Callbacks.OnWriteRequest?.Invoke(characteristic.Uuid.ToString(), value);
        }
      }

      public override void OnConnectionStateChange(BluetoothDevice device, [GeneratedEnum] ProfileState status,
        [GeneratedEnum] ProfileState newState)
      {
        base.OnConnectionStateChange(device, status, newState);
        Callbacks.OnConnectionChanged?.Invoke(newState == ProfileState.Connected);
      }

      public override void OnNotificationSent(BluetoothDevice device, [GeneratedEnum] GattStatus status)
      {
        base.OnNotificationSent(device, status);
        // todo implement
      }
    }
  }
}