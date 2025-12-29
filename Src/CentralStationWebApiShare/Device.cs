namespace CentralStationWebApi;

public class Device(CANMessage msg) 
{
    public uint DeviceId => msg.Device;
    public byte MajorVersion => msg.GetDataByte(9);
    public byte MinorVersion => msg.GetDataByte(10);
    public DeviceType DeviceType => (DeviceType)msg.GetDataUShort(11);

    public override int GetHashCode() => (DeviceId, MajorVersion, MinorVersion, DeviceType).GetHashCode();
    public override bool Equals(object? obj)
    {
        if (obj is Device device)
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
