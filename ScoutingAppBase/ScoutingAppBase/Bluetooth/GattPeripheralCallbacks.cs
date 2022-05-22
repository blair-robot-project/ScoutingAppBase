namespace ScoutingAppBase.Bluetooth
{
  public class GattPeripheralCallbacks
  {
    public ReadRequestCallback? OnReadRequest { get; set; }

    public WriteRequestCallback? OnWriteRequest { get; set; }

    public ConnectionChangedCallback? OnConnectionChanged { get; set; }

    public delegate void ReadRequestCallback(string characteristicUuid);

    public delegate void WriteRequestCallback(string characteristicUuid, byte[] value);

    public delegate void ConnectionChangedCallback(bool connected);
  }
}
