using System;

namespace ScoutingAppBase.Bluetooth
{
  public abstract class GattPeripheral : IDisposable
  {
    public abstract byte[] ReadCharacteristic(string uuid);
    
    public abstract void WriteCharacteristic(string uuid, byte[] value);

    public abstract void Dispose();
  }
}