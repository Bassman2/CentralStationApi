namespace CentralStationWebApi.Internal;

internal enum Command : byte
{
    SystemCommand = 0,
    Discovery = 1,
    Bind = 2,
    Versify = 3,
    LocoSpeed = 4,
    LocoDirection = 5,
    LocoFunction = 6,
    ReadConfig = 7,
    WriteConfig = 8,
    SwitchAccessories = 0x0B,
    S88Polling = 0x10,
    S88Event = 0x11,
    SoftwareVersion = 0x18,
    StatusData = 0x1D,
    ConfigData = 0x20,
    ConfigDataStream = 0x21,
    AutomaticTransmission = 0x30 
}
