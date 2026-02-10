namespace CentralStationWebApi;

public class DeviceData
{
    public DeviceData(uint deviceId, Version version, DeviceType deviceType = DeviceType.Application, uint serialNumber = 10000, string articleNumber = "10000", string deviceName = "ComputerApp")
    {
        DeviceId = deviceId;
        Version = version;
        DeviceType = deviceType;
        SerialNumber = serialNumber;
        ArticleNumber = articleNumber;
        DeviceName = deviceName;
    }
    
    public uint DeviceId { get; }
    public Version Version { get; }
    public DeviceType DeviceType { get; }
    public uint SerialNumber { get; }
    public string ArticleNumber { get; }
    public string DeviceName { get; }
}
