namespace CentralStationWebApi;

public class Controller(CANMessage msg) 
{
    // from SoftwareVersion
    public uint DeviceId => msg.Device;
    public byte MajorVersion => msg.GetDataByte(4);
    public byte MinorVersion => msg.GetDataByte(5);
    public DeviceType DeviceType => (DeviceType)msg.GetDataUShort(6);

    // from StatusData

    public bool HasStatusData { get; internal set; } = false;

    public byte Index { get; internal set; }

    public byte NumOfPackages { get; internal set; }

    // Package 1

    public byte NumOfMeasuredValues { get; internal set; }

    public byte NumOfConfigurationChannels { get; internal set; }

    public uint SerialNumber { get; internal set; }

    // Package 2

    public string? ArticleNumber { get; internal set; }

    // Package 3 - 5
    public string DeviceName { get; internal set; } = String.Empty;


    public void Update(StatusDataDevice statusDataDevice)
    {
        HasStatusData = true;
        Index = statusDataDevice.Index;
        NumOfPackages = statusDataDevice.NumOfPackages;
        NumOfMeasuredValues = statusDataDevice.NumOfMeasuredValues;
        NumOfConfigurationChannels = statusDataDevice.NumOfConfigurationChannels;
        SerialNumber = statusDataDevice.SerialNumber;
        ArticleNumber = statusDataDevice.ArticleNumber;
        DeviceName = statusDataDevice.DeviceName;
    }

    public override int GetHashCode() => (DeviceId, MajorVersion, MinorVersion, DeviceType).GetHashCode();
    public override bool Equals(object? obj)
    {
        if (obj is Controller device)
        {
            return 
                DeviceId == device.DeviceId && 
                MajorVersion == device.MajorVersion && 
                MinorVersion == device.MinorVersion && 
                DeviceType == device.DeviceType;
        }
        return false;
    }
    
}
