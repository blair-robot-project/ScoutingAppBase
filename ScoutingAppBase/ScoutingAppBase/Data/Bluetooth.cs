using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScoutingAppBase.Data
{
  internal sealed class Bluetooth : IDisposable
  {
    /// <summary>
    /// How long to wait (in ms) to connect
    /// </summary>
    public static int ConnTimeout = 5000;

    public static readonly string GuidBase = "0000-1000-8000-00805f9b34fb";
    /// <summary>
    /// The 16-bit GUID of the first characteristic. Yes, this is arbitrary
    /// </summary>
    public static readonly int StartGuid = 0x2A91;

    /// <summary>
    /// The name of the central bluetooth device.
    /// </summary>
    private readonly string CentralName;

    /// <summary>
    /// The central device's GUID for connecting faster. -1 if GUID is not known.
    /// </summary>
    private readonly Guid? CentralGuid;

    /// <summary>
    /// The central if currently connected to it
    /// </summary>
    // todo check if this is thread-safe
    private IDevice? Central;

    private readonly List<IDevice> Devices = new List<IDevice>();

    public bool IsConnected { get => Central != null; }

    private IAdapter Adapter { get => CrossBluetoothLE.Current.Adapter; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="centralName">The name of the central device's bluetooth adapter</param>
    /// <param name="centralGuid">The central device's adapter's GUID, for connecting faster (optional)</param>
    public Bluetooth(string centralName, Guid? centralGuid = null)
    {
      (CentralName, CentralGuid) = (centralName, centralGuid);
      Initialize();
    }

    private async void Initialize()
    {
      var ble = CrossBluetoothLE.Current;
      var adapter = ble.Adapter;

      await SearchDevices();
    }

    /// <summary>
    /// Send some data to the central device
    /// </summary>
    /// <returns>Whether it succeeded</returns>
    public async Task<bool> Send(byte[] data)
    {
      var connected = await TryConnect();
      if (!connected) return false;

      var central = Central!;

      // The index in the array of data
      int ind = 0;
      foreach (var service in await central.GetServicesAsync())
      {
        service.GetCharacteristicAsync(To128BitGuid(StartGuid));
        foreach (var characteristic in await service.GetCharacteristicsAsync())
        {
          if (characteristic.Id == null)
          {
            await characteristic.WriteAsync(data);
          }
        }
      }

      // todo implement
      return false;
    }

    /// <summary>
    /// Convert an integer representing a 16-bit GUID to a 128-bit one
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    private Guid To128BitGuid(int prefix)
    {
      return Guid.Parse(prefix.ToString("X8") + "-" + GuidBase);
    }

    private async Task SearchDevices()
    {
      // todo ensure these three methods don't mess with TryConnect
      Adapter.DeviceDiscovered += (sender, args) =>
      {
        Devices.Add(args.Device);
        if (args.Device.Name == CentralName)
        {
          Central = args.Device;
        }
      };
      Adapter.DeviceDisconnected += (sender, args) => Disconnected(args.Device);
      Adapter.DeviceConnectionLost += (sender, args) => Disconnected(args.Device);

      var cts = new CancellationTokenSource();
      cts.CancelAfter(Bluetooth.ConnTimeout);
      await Adapter.StartScanningForDevicesAsync(cancellationToken: cts.Token);
    }

    private void Disconnected(IDevice device)
    {
      Devices.Remove(device);

      if (device == Central)
      {
        // todo notify user if central disconnected
        Central = null;
      }
    }

    private async Task<bool> TryConnect()
    {
      if (IsConnected) return true;
      try
      {
        if (CentralGuid != null)
        {
          var cts = new CancellationTokenSource();
          cts.CancelAfter(ConnTimeout);
          // todo figure out why this needs a cast
          Central = await Adapter.ConnectToKnownDeviceAsync((Guid)CentralGuid!, cancellationToken: cts.Token);
          return true;
        } else
        {
          var central = Devices.Find(x => x.Name == CentralName);
          if (central == null)
          {
            await SearchDevices();
            central = Devices.Find(x => x.Name == CentralName);
          }
          // If it's still not found, can't connect
          if (central == null) return false;

          var cts = new CancellationTokenSource();
          cts.CancelAfter(ConnTimeout);
          var task = Adapter.ConnectToDeviceAsync(central, cancellationToken: cts.Token);
          await task;
          if (task.IsCanceled)
          {
            return false;
          } else
          {
            Central = central;
            return true;
          }
        }
      } catch (DeviceConnectionException)
      {
        return false;
      }
    }

    private void OnConnect()
    {

    }

    public void Dispose()
    {
      if (Central != null) Adapter.DisconnectDeviceAsync(Central);
      Central?.Dispose();
    }
  }
}
