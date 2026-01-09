namespace CentralStationWebApi;

[DebuggerDisplay("StatusDataDevice {DeviceId} {ArticleNumber} {DeviceName}")]
public class StatusDataValue
{
    internal StatusDataValue(uint device, byte index, byte numOfPackages, DataCollector col)
    {
        DeviceId = device;
        Index = index;
        NumOfPackages = numOfPackages;

        // package 1
        Channel = col.ReadByte();
        Potency = col.ReadByte();
        Color1 = col.ReadByte();
        Color2 = col.ReadByte();
        Color3 = col.ReadByte();
        Color4 = col.ReadByte();
        ZeroPoint =  col.ReadUInt16();
        // package 2
        End1 = col.ReadUInt16();
        End2 = col.ReadUInt16();
        End3 = col.ReadUInt16();
        End4 = col.ReadUInt16();
        // package 3, 4, 5
        MeasurementDesignation = col.ReadString();
        Start = col.ReadString();
        End = col.ReadString();
        Unit = col.ReadString();
    }

    public uint DeviceId { get; internal set; }

    public byte Index { get; internal set; }

    public byte NumOfPackages { get; internal set; }



    // Package 1

    public byte Channel { get; internal set; }

    public byte Potency { get; internal set; }

    public byte Color1 { get; internal set; }

    public byte Color2 { get; internal set; }

    public byte Color3 { get; internal set; }

    public byte Color4 { get; internal set; }

    public ushort ZeroPoint { get; internal set; }

    // Package 2

    public ushort End1 { get; internal set; }

    public ushort End2 { get; internal set; }

    public ushort End3 { get; internal set; }

    public ushort End4 { get; internal set; }

    // Package 3 - 5

    public string MeasurementDesignation { get; internal set; } = string.Empty;

    public string Start { get; internal set; } = string.Empty;

    public string End { get; internal set; } = string.Empty;

    public string Unit { get; internal set; } = string.Empty;
}
