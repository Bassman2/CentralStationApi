namespace CentralStationApi;

/// <summary>
/// Represents device identification and version information for a client connecting to the Central Station.
/// Used to identify the client application on the CAN bus network.
/// </summary>
/// <remarks>
/// When connecting to the Central Station, the client must provide device data to identify itself
/// on the network. This information is broadcast in response to software version requests and
/// allows the Central Station to distinguish between different connected devices and applications.
/// </remarks>
public class DeviceData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceData"/> class with the specified identification information.
    /// </summary>
    /// <param name="deviceId">The unique device identifier (UID) for this client. Should be unique on the network.</param>
    /// <param name="version">The software version of the client application.</param>
    /// <param name="deviceType">The type of device. Default is <see cref="DeviceType.Application"/>.</param>
    /// <param name="serialNumber">The serial number of the device. Default is 10000.</param>
    /// <param name="articleNumber">The article/product number of the device. Default is "10000".</param>
    /// <param name="deviceName">The display name of the device. Default is "ComputerApp".</param>
    /// <remarks>
    /// The <paramref name="deviceId"/> is used to generate a hash value for CAN message addressing.
    /// Default values are suitable for most client applications connecting to the Central Station.
    /// </remarks>
    public DeviceData(uint deviceId, Version version, DeviceType deviceType = DeviceType.Application, uint serialNumber = 10000, string articleNumber = "10000", string deviceName = "ComputerApp")
    {
        DeviceId = deviceId;
        Version = version;
        DeviceType = deviceType;
        SerialNumber = serialNumber;
        ArticleNumber = articleNumber;
        DeviceName = deviceName;
    }

    /// <summary>
    /// Gets the unique device identifier (UID).
    /// </summary>
    /// <value>
    /// A 32-bit unsigned integer that uniquely identifies this device on the CAN bus network.
    /// This value is used to generate the hash value for CAN message addressing.
    /// </value>
    public uint DeviceId { get; }

    /// <summary>
    /// Gets the software version of the client application.
    /// </summary>
    /// <value>
    /// The version information containing major and minor version numbers.
    /// </value>
    public Version Version { get; }

    /// <summary>
    /// Gets the type of device.
    /// </summary>
    /// <value>
    /// The <see cref="DeviceType"/> indicating the category of this device (e.g., Application, Central Station, Booster).
    /// </value>
    public DeviceType DeviceType { get; }

    /// <summary>
    /// Gets the serial number of the device.
    /// </summary>
    /// <value>
    /// A unique serial number for this device instance.
    /// </value>
    public uint SerialNumber { get; }

    /// <summary>
    /// Gets the article or product number of the device.
    /// </summary>
    /// <value>
    /// The article number string, typically corresponding to a Märklin product number.
    /// </value>
    public string ArticleNumber { get; }

    /// <summary>
    /// Gets the display name of the device.
    /// </summary>
    /// <value>
    /// A human-readable name for the device, used for identification in the Central Station interface.
    /// </value>
    public string DeviceName { get; }
}
