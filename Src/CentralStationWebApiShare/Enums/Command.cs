namespace CentralStationWebApi;

public enum Command : byte
{
    SystemCommand = 0,
    Discovery = 1,
    Bind = 2,
    Verify = 3,
    LocoSpeed = 4,
    LocoDirection = 5,
    LocoFunction = 6,
    ReadConfig = 7,
    WriteConfig = 8,
    SwitchAccessories = 0x0B,
    S88Polling = 0x10,
    S88Event = 0x11,
    SX1Event = 0x12,
    SoftwareVersion = 0x18,
    UpdateOffer = 0x19,
    ReadConfigData = 0x1A,
    BootloaderCANBound = 0x1B,
    BootloaderRailBound = 0x1C,
    StatusData = 0x1D,
    ConfigData = 0x20,
    ConfigDataStream = 0x21,
    DataStream6021Adapter = 0x22,
    AutomaticTransmission = 0x30 
}
