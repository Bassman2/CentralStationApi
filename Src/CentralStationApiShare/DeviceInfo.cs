namespace CentralStationApi;

/// <summary>
/// Represents detailed information about a device on the Central Station network.
/// Contains identification, capabilities, and configuration information for boosters, decoders, and other devices.
/// </summary>
/// <remarks>
/// Device information is retrieved using the <see cref="Command.StatusData"/> command.
/// This class provides information about a device's capabilities (number of measurement channels,
/// configuration options) as well as identification data (serial number, article number, name).
/// </remarks>
public class DeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceInfo"/> class from a CAN message collection.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the device.</param>
    /// <param name="col">The CAN message collector containing the device information data.</param>
    /// <remarks>
    /// This constructor parses the status data response from the device, extracting capabilities,
    /// serial number, article number, and device name from the message stream.
    /// </remarks>
    internal DeviceInfo(uint deviceId, CanMessageCollector col)
    {
        DeviceId = deviceId;
        col.SetPositionToStart();
        NumOfMeasuredValues = col.ReadByte();
        NumOfConfigurationChannels = col.ReadByte();
        col.ReadByte();
        col.ReadByte();
        SerialNumber = col.ReadUInt32();
        ArticleNumber = col.ReadString(8);
        DeviceName = col.ReadString();
        
    }

    /// <summary>
    /// Gets or sets the unique device identifier (UID).
    /// </summary>
    /// <value>
    /// A 32-bit unsigned integer that uniquely identifies this device on the CAN bus network.
    /// </value>
    public uint DeviceId { get; internal set; }

    /// <summary>
    /// Gets or sets the number of measurement channels available on the device.
    /// </summary>
    /// <value>
    /// The count of measurement channels that can report values such as voltage, current, or temperature.
    /// Used to determine how many <see cref="DeviceMeasurement"/> requests can be made.
    /// </value>
    public byte NumOfMeasuredValues { get; internal set; }

    /// <summary>
    /// Gets or sets the number of configuration channels available on the device.
    /// </summary>
    /// <value>
    /// The count of configuration channels that can be queried or modified for device settings.
    /// </value>
    public byte NumOfConfigurationChannels { get; internal set; }

    /// <summary>
    /// Gets or sets the serial number of the device.
    /// </summary>
    /// <value>
    /// A unique serial number assigned to this specific device instance by the manufacturer.
    /// </value>
    public uint SerialNumber { get; internal set; }

    /// <summary>
    /// Gets or sets the article or product number of the device.
    /// </summary>
    /// <value>
    /// The Märklin article number identifying the device model (e.g., "60214" for a booster).
    /// Maximum length is 8 characters.
    /// </value>
    public string ArticleNumber { get; internal set; } = String.Empty;

    /// <summary>
    /// Gets or sets the display name of the device.
    /// </summary>
    /// <value>
    /// A human-readable name for the device, which may be user-configurable or set by the manufacturer.
    /// Used for identification in the Central Station interface.
    /// </value>
    public string DeviceName { get; internal set; } = String.Empty;
}
