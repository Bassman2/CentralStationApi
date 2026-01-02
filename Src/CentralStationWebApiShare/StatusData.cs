namespace CentralStationWebApi;

public class StatusData
{
    public uint DeviceId { get; internal set; }

    public byte NumOfMeasuredValues { get; internal set; }

    public byte NumOfConfigurationChannels { get; internal set; }

    public uint SerialNumber { get; internal set; }

    public string? ArticleNumber { get; internal set; }

    public string DeviceName { get; internal set; } = String.Empty;
}
