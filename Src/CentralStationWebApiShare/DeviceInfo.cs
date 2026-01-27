namespace CentralStationWebApi;

public class DeviceInfo
{
    internal DeviceInfo(DataCollector col)
    {
        col.SetPositionToStart();
        NumOfMeasuredValues = col.ReadByte();
        NumOfConfigurationChannels = col.ReadByte();
        col.ReadByte();
        col.ReadByte();
        SerialNumber = col.ReadUInt32();
        ArticleNumber = col.ReadString(8);
        DeviceName = col.ReadString();
        
    }

    public byte NumOfMeasuredValues { get; internal set; }

    public byte NumOfConfigurationChannels { get; internal set; }

    public uint SerialNumber { get; internal set; }

    public string ArticleNumber { get; internal set; } = String.Empty;

    public string DeviceName { get; internal set; } = String.Empty;
}
