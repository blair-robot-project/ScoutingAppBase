using System;
using System.Collections.Generic;
using System.Text;

namespace ScoutingAppBase.Bluetooth
{
  public abstract class GattPeripheral : IDisposable
  {
    public readonly List<GattService> Services;

    public readonly GattPeripheralCallbacks Callbacks;

    protected GattPeripheral(List<GattService> services, GattPeripheralCallbacks callbacks)
    {
      Services = services;
      Callbacks = callbacks;
    }

    public abstract byte[] ReadCharacteristic(string uuid);
    
    public abstract void WriteCharacteristic(string uuid, byte[] value);

    public abstract void Dispose();
  }
}