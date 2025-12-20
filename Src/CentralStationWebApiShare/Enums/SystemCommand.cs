namespace CentralStationWebApi;

public enum SystemCommand : byte
{
    Stop = 0x00,
    Go = 0x01,
    Halt = 0x02,
    LocoHalt = 0x03,
    LocoCycleStop = 0x04,
    LocoDataProtocol = 0x05,
    SwitchingTime = 0x06,
    FastRead = 0x07,
    TrackProtocol = 0x08,
    NewRegistrationCounter = 0x09,
    Overload = 0x0A,
    Status = 0x0B,
    Identifier = 0x0C,
    MfxSeek = 0x30,
    Reset = 0x80,
}
