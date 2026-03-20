namespace CentralStationWebApi;

/// <summary>
/// Represents a device discovered on the Central Station network.
/// Contains identification, version, and type information for devices such as control stations,
/// boosters, decoders, and other hardware components.
/// </summary>
/// <remarks>
/// Devices are discovered through the <see cref="Command.SoftwareVersion"/> broadcast request,
/// which prompts all devices on the CAN bus to respond with their identification information.
/// This class is used by <see cref="CentralStation.GetAllDevicesAsync"/> to enumerate all
/// connected devices on the network.
/// </remarks>
public class Device
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Device"/> class with the specified identification information.
    /// </summary>
    /// <param name="deviceId">The unique device identifier (UID) of the device.</param>
    /// <param name="version">The software/firmware version of the device.</param>
    /// <param name="deviceType">The type of device (control station, booster, decoder, etc.).</param>
    public Device(uint deviceId, Version version, DeviceType deviceType) 
    {
        DeviceId = deviceId;
        Version = version;
        DeviceType = deviceType;
        DeviceTypeName = DeviceType.GetDescription();
        IconUri = DeviceType.GetFileNamePath(CentralStation.GuiUri);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Device"/> class from a CAN message.
    /// </summary>
    /// <param name="msg">The <see cref="CanMessage"/> containing the device information from a software version response.</param>
    /// <remarks>
    /// This internal constructor parses device information from <see cref="Command.SoftwareVersion"/>
    /// response messages, extracting the device ID, version (major.minor), and device type.
    /// </remarks>
    internal Device(CanMessage msg) : this(msg.DeviceId, new Version(msg.GetDataByte(4), msg.GetDataByte(5)), (DeviceType)msg.GetDataUShort(6))
    { }

    /// <summary>
    /// Gets or sets the unique device identifier (UID).
    /// </summary>
    /// <value>
    /// A 32-bit unsigned integer that uniquely identifies this device on the CAN bus network.
    /// </value>
    public uint DeviceId { get; internal set; }

    /// <summary>
    /// Gets or sets the software or firmware version of the device.
    /// </summary>
    /// <value>
    /// The version information with major and minor version numbers.
    /// </value>
    public Version Version { get; internal set; }

    /// <summary>
    /// Gets or sets the type of device.
    /// </summary>
    /// <value>
    /// The <see cref="DeviceType"/> indicating the device category (e.g., Central Station, Booster, Mobile Station).
    /// </value>
    public DeviceType DeviceType { get; internal set; }

    /// <summary>
    /// Gets or sets the human-readable name of the device type.
    /// </summary>
    /// <value>
    /// A descriptive name derived from the <see cref="DeviceType"/> enum's description attribute,
    /// suitable for display in user interfaces.
    /// </value>
    public string DeviceTypeName { get; internal set; }

    /// <summary>
    /// Gets or sets the URI to the icon image for this device type.
    /// </summary>
    /// <value>
    /// A URI pointing to the icon image file on the Central Station web server,
    /// or null if no icon is available for this device type.
    /// </value>
    public Uri? IconUri { get; internal set; }
}
