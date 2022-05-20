using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ScoutingAppBase.Data;
using ScoutingAppBase.Data.Bluetooth;

namespace ScoutingAppBase.Droid.Data.Bluetooth
{
  internal class DroidGattPeripheral : IGattPeripheral
  {
    private readonly BluetoothManager Manager;
    private readonly BluetoothAdapter Adapter;
    private readonly BluetoothGattServer GattServer;

    private readonly EventConfig Config;

    public DroidGattPeripheral(EventConfig config)
    {
      Config = config;
      Manager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
      Adapter = Manager.Adapter;

      GattServer = Manager.OpenGattServer(Application.Context, new GattServerCallbackImpl())!;

      // Add our custom service and characteristics
      var serviceUuid = UUID.FromString(config.ServiceUuid);
      var service = new BluetoothGattService(serviceUuid, GattServiceType.Primary);

      foreach (var fieldConfig in Config.FieldConfigs)
      {
        var characteristic = new BluetoothGattCharacteristic(
          UUID.FromString(fieldConfig.CharUuid),
          GattProperty.Read | GattProperty.Write | GattProperty.Notify,
          GattPermission.Read | GattPermission.Write
        );
        service.AddCharacteristic(characteristic);
      }

      GattServer.AddService(service);

      // Start advertising
      var adSettings = new AdvertiseSettings.Builder()
        .SetConnectable(true)
        .SetTimeout(0)
        .SetAdvertiseMode(AdvertiseMode.LowLatency)
        .SetTxPowerLevel(AdvertiseTx.PowerHigh)
        .Build();

      var adData = new AdvertiseData.Builder()
        .SetIncludeDeviceName(true)
        .SetIncludeTxPowerLevel(true)
        .AddServiceUuid(new ParcelUuid(serviceUuid))
        .Build();

      Adapter.BluetoothLeAdvertiser.StartAdvertising(adSettings, adData, new AdvertiseCallbackImpl());
    }

    public void Dispose()
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