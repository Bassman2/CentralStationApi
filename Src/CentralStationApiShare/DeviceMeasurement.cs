namespace CentralStationApi;

/// <summary>
/// Represents measurement data from a Central Station device channel.
/// Contains information about voltage, current, temperature, or other physical measurements
/// including range definitions and display settings.
/// </summary>
public class DeviceMeasurement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceMeasurement"/> class from a CAN message collection.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the device providing the measurement.</param>
    /// <param name="index">The measurement channel index.</param>
    /// <param name="col">The CAN message collector containing the measurement data.</param>
    internal DeviceMeasurement(uint deviceId, byte index, CanMessageCollector col)
    {
        DeviceId = deviceId;
        Index = index;

        col.SetPositionToStart();
        
        Channel = col.ReadByte();
        ValuePower = col.ReadSByte();
        ColorRange1 = col.ReadByte();
        ColorRange2 = col.ReadByte();
        ColorRange3 = col.ReadByte();
        ColorRange4 = col.ReadByte();
        ZeroPoint = col.ReadUInt16();
        EndRange1 = col.ReadUInt16();
        EndRange2 = col.ReadUInt16();
        EndRange3 = col.ReadUInt16();
        EndRange4 = col.ReadUInt16();
        Name = col.ReadString();
        Start = col.ReadString();
        End = col.ReadString();
        Unit = col.ReadString();
    }

    /// <summary>
    /// Gets or sets the unique identifier of the device providing this measurement.
    /// </summary>
    public uint DeviceId { get; internal set; }

    /// <summary>
    /// Gets or sets the measurement channel index on the device.
    /// </summary>
    public byte Index { get; internal set; }

    /// <summary>
    /// Gets or sets the channel number for this measurement.
    /// </summary>
    public byte Channel { get; internal set; }

    /// <summary>
    /// Gets or sets the power/exponent value for scaling the measurement.
    /// Used to calculate the actual value (e.g., 10^ValuePower).
    /// </summary>
    public sbyte ValuePower { get; internal set; }

    /// <summary>
    /// Gets or sets the color code for display range 1.
    /// Used for visual representation of the measurement value.
    /// </summary>
    public byte ColorRange1 { get; internal set; }

    /// <summary>
    /// Gets or sets the color code for display range 2.
    /// Used for visual representation of the measurement value.
    /// </summary>
    public byte ColorRange2 { get; internal set; }

    /// <summary>
    /// Gets or sets the color code for display range 3.
    /// Used for visual representation of the measurement value.
    /// </summary>
    public byte ColorRange3 { get; internal set; }

    /// <summary>
    /// Gets or sets the color code for display range 4.
    /// Used for visual representation of the measurement value.
    /// </summary>
    public byte ColorRange4 { get; internal set; }

    /// <summary>
    /// Gets or sets the zero point reference value for the measurement scale.
    /// </summary>
    public ushort ZeroPoint { get; internal set; }

    /// <summary>
    /// Gets or sets the end value of display range 1.
    /// Defines the upper limit of the first color-coded range.
    /// </summary>
    public ushort EndRange1 { get; internal set; }

    /// <summary>
    /// Gets or sets the end value of display range 2.
    /// Defines the upper limit of the second color-coded range.
    /// </summary>
    public ushort EndRange2 { get; internal set; }

    /// <summary>
    /// Gets or sets the end value of display range 3.
    /// Defines the upper limit of the third color-coded range.
    /// </summary>
    public ushort EndRange3 { get; internal set; }

    /// <summary>
    /// Gets or sets the end value of display range 4.
    /// Defines the upper limit of the fourth color-coded range.
    /// </summary>
    public ushort EndRange4 { get; internal set; }

    /// <summary>
    /// Gets or sets the name or description of the measurement (e.g., "Voltage", "Current", "Temperature").
    /// </summary>
    public string Name { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the label text for the start of the measurement scale.
    /// </summary>
    public string Start { get; internal set; } = String.Empty;

    /// <summary>
    /// Gets or sets the label text for the end of the measurement scale.
    /// </summary>
    public string End { get; internal set; } = String.Empty;

    /// <summary>
    /// Gets or sets the unit of measurement (e.g., "V", "mA", "°C").
    /// </summary>
    public string Unit { get; internal set; } = String.Empty;
}
