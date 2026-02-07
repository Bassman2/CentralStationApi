namespace CentralStationWebApi;

public class Device
{
    public Device(uint deviceId, System.Version version, DeviceType deviceType) 
    {
        DeviceId = deviceId;
        Version = version;
        DeviceType = deviceType;
        DeviceTypeName = DeviceType.GetDescription();
        IconUri = DeviceType.GetFileNamePath(CentralStation.GuiUri);
    }
    internal Device(CanMessage msg) : this(msg.DeviceId, new System.Version(msg.GetDataByte(4), msg.GetDataByte(5)), (DeviceType)msg.GetDataUShort(6))
    { }

    public uint DeviceId { get; internal set; }
    public System.Version Version { get; internal set; }    
    public DeviceType DeviceType { get; internal set; }
    public string DeviceTypeName { get; internal set; }

    public Uri? IconUri { get; internal set; }
}
