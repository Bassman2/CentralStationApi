namespace CentralStationWebApi;

[Flags]
public enum TrackProtocol : byte
{
    MM2 = 0x01,
    MFX = 0x02,
    DCC = 0x04
}
