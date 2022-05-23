using System.Collections.Generic;
using System.Linq;

namespace ScoutingAppBase.Bluetooth
{
  public sealed class GattChar
  {
    public readonly string Uuid;
    public readonly ISet<Property> Properties;
    public readonly ISet<Permission> Permissions;

    public GattChar(string uuid, IEnumerable<Property> properties, IEnumerable<Permission> permissions)
    {
      (Uuid, Properties, Permissions) = (uuid, properties.ToHashSet(), permissions.ToHashSet());
    }

    public enum Property
    {
      Broadcast,
      Read,
      WriteNoResponse,
      Write,
      Notify,
      Indicate,
      SignedWrite,
      ExtendProps
    }

    public enum Permission
    {
      Read,
      ReadEncrypted,
      ReadEncryptedMitm,
      Write,
      WriteEncrypted,
      WriteEncryptedMitm,
      WriteSigned,
      WriteSignedMitm
    }
  }
}
