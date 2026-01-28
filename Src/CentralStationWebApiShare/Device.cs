namespace CentralStationWebApi;

public class Device
{
    internal Device(CANMessage msg)
    {
        DeviceId = msg.Device;
        Version = new System.Version(msg.GetDataByte(4), msg.GetDataByte(5));
        DeviceType = (DeviceType)msg.GetDataUShort(6);
        DeviceTypeName = DeviceType.GetDescription();
        IconUri = DeviceType.GetFileNamePath(CentralStation.GuiUri); 
    }
    public uint DeviceId { get; internal set; }
    public System.Version Version { get; internal set; }    
    public DeviceType DeviceType { get; internal set; }
    public string DeviceTypeName { get; internal set; }

    public Uri? IconUri { get; internal set; }
}
