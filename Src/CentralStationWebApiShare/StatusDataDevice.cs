namespace CentralStationWebApi;

[DebuggerDisplay("StatusDataDevice {DeviceId} {ArticleNumber} {DeviceName}")]
public class StatusDataDevice
{
    internal StatusDataDevice(uint device, byte index, byte numOfPackages, DataCollector col)
    {
        DeviceId = device;
        Index = index;
        NumOfPackages = numOfPackages;

        col.SetPositionToStart();
        
        // package 1
        NumOfMeasuredValues = col.ReadByte(); 
        NumOfConfigurationChannels = col.ReadByte();
        col.ReadByte();
        col.ReadByte();
        SerialNumber = col.ReadUInt32();

        // package 2
        ArticleNumber = col.ReadString(8);

        // package 3, ...
        DeviceName = col.ReadString();
    }
    
    public uint DeviceId { get; internal set; }

    public byte Index { get; internal set; }

    public byte NumOfPackages { get; internal set; }

    public byte NumOfMeasuredValues { get; internal set; }

    public byte NumOfConfigurationChannels { get; internal set; }

    public uint SerialNumber { get; internal set; }

    public string? ArticleNumber { get; internal set; }

    public string DeviceName { get; internal set; } = String.Empty;

    public StatusDataValue[]? Values { get; internal set; }
}
