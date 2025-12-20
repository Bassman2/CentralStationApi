namespace CentralStationWebApi;

public enum SystemCommand : byte
{
    SystemStop = 0,
    SystemGo = 1,
    SystemHalt = 2,

    SystemReset = 0x80,
}
