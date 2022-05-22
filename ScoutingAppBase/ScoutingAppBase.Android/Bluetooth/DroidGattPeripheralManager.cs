using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using System.Collections.Generic;
using Xamarin.Forms;

using ScoutingAppBase.Bluetooth;
using System;
using Android.OS;
using Java.Util;

[assembly: Dependency(typeof(ScoutingAppBase.Droid.Bluetooth.DroidGattPeripheralManager))]
namespace ScoutingAppBase.Droid.Bluetooth
{
  internal class DroidGattPeripheralManager : IGattPeripheralManager
  {
    private readonly BluetoothManager Manager;

    public DroidGattPeripheralManager()
    {
      Manager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
    }

    public GattPeripheral Create(
      List<GattService> services,
      GattPeripheralCallbacks callbacks) => new DroidGattPeripheral(Manager, services, callbacks);

    public void StartAdvertising(GattAdOptions adData)
    {
      // todo don't hardcode stuff like this
      var adSettingsBuilder = new AdvertiseSettings.Builder()
        .SetConnectable(true)
        .SetAdvertiseMode(AdvertiseMode.LowLatency)
        .SetTimeout(adData.Timeout);
      var adDataBuilder = new AdvertiseData.Builder();

      if (adData.IncludeDeviceName)
      {
        adDataBuilder.SetIncludeDeviceName(true);
      }

      if (adData.PowerLevel != null)
      {
        adSettingsBuilder.SetTxPowerLevel(
          adData.PowerLevel switch
          {
            GattAdOptions.TxPowerLevel.PowerUltraLow => AdvertiseTx.PowerUltraLow,
            GattAdOptions.TxPowerLevel.PowerLow => AdvertiseTx.PowerLow,
            GattAdOptions.TxPowerLevel.PowerMedium => AdvertiseTx.PowerMedium,
            GattAdOptions.TxPowerLevel.PowerHigh => AdvertiseTx.PowerHigh,
            _ => throw new ArgumentOutOfRangeException()
          }
        );
        adDataBuilder.SetIncludeTxPowerLevel(true);
      }

      foreach (var serviceUuid in adData.ServiceUuids)
      {
        adDataBuilder.AddServiceUuid(new ParcelUuid(UUID.FromString(serviceUuid)));
      }

      if (adData.ManufacturerSpecificData != null)
      {
        foreach (var (manufacturerId, data) in adData.ManufacturerSpecificData)
        {
          adDataBuilder.AddManufacturerData(manufacturerId, data);
        }
      }

      if (adData.ServiceData != null)
      {
        foreach (var (uuid, data) in adData.ServiceData)
        {
          adDataBuilder.AddServiceData(new ParcelUuid(UUID.FromString(uuid)), data);
        }
      }

      Manager.Adapter.BluetoothLeAdvertiser.StartAdvertising(
        adSettingsBuilder.Build(),
        adDataBuilder.Build(),
        null);
    }

    public void StopAdvertising()
    {
      Manager.Adapter.BluetoothLeAdvertiser.StopAdvertising(null);
    }
  }
}
