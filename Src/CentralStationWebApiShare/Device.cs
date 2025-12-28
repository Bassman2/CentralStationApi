namespace CentralStationWebApi;

public class Device(uint deviceId, byte majorVersion, byte minorVersion, ushort deviceType)
{
    public uint DeviceId => deviceId;
    public byte MajorVersion => majorVersion;
    public byte MinorVersion => minorVersion;
    public ushort DeviceType => deviceType;
}
