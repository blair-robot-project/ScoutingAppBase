namespace ScoutingAppBase.Bluetooth
{
  /// <summary>
  /// Holds callbacks for various events that a peripheral gets
  /// </summary>
  public sealed class GattPeripheralCallbacks
  {
    /// <summary>
    /// Triggered when a central device wants to read a characteristic
    /// </summary>
    public ReadRequestCallback? OnReadRequest { get; set; }

    /// <summary>
    /// Triggered when a central device wants to write to a characteristic
    /// </summary>
    public WriteRequestCallback? OnWriteRequest { get; set; }

    // todo actually implement detecting connection state
    /// <summary>
    /// Triggered when the central connects or disconnects
    /// </summary>
    public ConnectionChangedCallback? OnConnectionChanged { get; set; }

    public delegate void ReadRequestCallback(string characteristicUuid);

    public delegate void WriteRequestCallback(string characteristicUuid, byte[] value);

    public delegate void ConnectionChangedCallback(bool connected);
  }
}
