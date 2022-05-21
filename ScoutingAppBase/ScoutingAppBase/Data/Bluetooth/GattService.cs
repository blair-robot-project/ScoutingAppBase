using System;
using System.Collections.Generic;
using System.Text;

namespace ScoutingAppBase.Data.Bluetooth
{
  public class GattService
  {
    public readonly string Uuid;
    public readonly Type ServiceType;
    public readonly ISet<GattCharacteristic> Characteristics;

    public GattService(string uuid, Type serviceType, ISet<GattCharacteristic> characteristics)
    {
      (Uuid, ServiceType, Characteristics) = (uuid, serviceType, characteristics);
    }

    public enum Type
    {
      Primary, Secondary
    }
  }
}
