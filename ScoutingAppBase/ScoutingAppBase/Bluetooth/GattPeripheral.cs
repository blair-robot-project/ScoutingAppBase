using System;
using System.Collections.Generic;
using System.Text;

namespace ScoutingAppBase.Bluetooth
{
  public abstract class GattPeripheral : IDisposable
  {
    public readonly List<GattService> Services;

    public readonly GattPeripheralCallbacks Callbacks;

    public GattPeripheral(List<GattService> services, GattPeripheralCallbacks callbacks)
    {
      Services = services;
      Callbacks = callbacks;
    }

    public abstract void Dispose();
  }
}
