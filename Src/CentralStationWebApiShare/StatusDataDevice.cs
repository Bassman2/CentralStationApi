namespace CentralStationWebApi;

public class StatusDataDevice
{
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
