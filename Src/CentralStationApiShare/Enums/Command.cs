namespace CentralStationApi;

/// <summary>
/// Represents the command types used in CAN messages for communication with the Märklin Central Station.
/// Each command corresponds to a specific operation or request in the CAN bus protocol.
/// </summary>
public enum Command : byte
{
    /// <summary>
    /// System-level commands (Stop, Go, Halt, etc.).
    /// Used for controlling overall system state and track power.
    /// </summary>
    SystemCommand = 0,

    /// <summary>
    /// Device discovery command.
    /// Used to discover devices on the CAN bus network.
    /// </summary>
    Discovery = 1,

    /// <summary>
    /// Bind command for device pairing.
    /// Used to establish a connection between devices.
    /// </summary>
    Bind = 2,

    /// <summary>
    /// Verify command for device verification.
    /// Used to confirm device identity and status.
    /// </summary>
    Verify = 3,

    /// <summary>
    /// Locomotive velocity command.
    /// Used to get or set the speed of a locomotive (0-1000).
    /// </summary>
    LocoVelocity = 4,

    /// <summary>
    /// Locomotive direction command.
    /// Used to get or set the direction of travel for a locomotive.
    /// </summary>
    LocoDirection = 5,

    /// <summary>
    /// Locomotive function command.
    /// Used to control locomotive functions (lights, sound, etc.).
    /// </summary>
    LocoFunction = 6,

    /// <summary>
    /// Read configuration command.
    /// Used to read configuration values from devices.
    /// </summary>
    ReadConfig = 7,

    /// <summary>
    /// Write configuration command.
    /// Used to write configuration values to devices.
    /// </summary>
    WriteConfig = 8,

    /// <summary>
    /// Switch accessories command.
    /// Used to control turnouts, signals, and other track accessories.
    /// </summary>
    SwitchAccessories = 0x0B,

    /// <summary>
    /// Accessory configuration command (not in official specification).
    /// </summary>
    /// <remarks>This command is not part of the official Märklin Central Station specification.</remarks>
    ACC_CONFIG = 0x0C,

    /// <summary>
    /// S88 polling command (obsolete).
    /// Used to poll S88 feedback bus modules for contact status.
    /// </summary>
    S88Polling = 0x10,

    /// <summary>
    /// S88 event command.
    /// Used to register for and receive S88 feedback events.
    /// </summary>
    S88Event = 0x11,

    /// <summary>
    /// SX1 event command.
    /// Used for Selectrix (SX1) bus events.
    /// </summary>
    SX1Event = 0x12,

    /// <summary>
    /// Software version command.
    /// Used to request or report device software version information.
    /// </summary>
    SoftwareVersion = 0x18,

    /// <summary>
    /// Update offer command.
    /// Used to offer firmware updates to devices.
    /// </summary>
    UpdateOffer = 0x19,

    /// <summary>
    /// Read configuration data command.
    /// Used to read configuration data from devices.
    /// </summary>
    ReadConfigData = 0x1A,

    /// <summary>
    /// Bootloader CAN bound command.
    /// Used for bootloader operations over the CAN bus.
    /// </summary>
    BootloaderCANBound = 0x1B,

    /// <summary>
    /// Bootloader rail bound command.
    /// Used for bootloader operations over the track/rail connection.
    /// </summary>
    BootloaderRailBound = 0x1C,

    /// <summary>
    /// Status data command.
    /// Used to request or report device status and measurement data.
    /// </summary>
    StatusData = 0x1D,

    /// <summary>
    /// Configuration data request command.
    /// Used to request configuration files from the Central Station.
    /// </summary>
    ConfigDataRequest = 0x20,

    /// <summary>
    /// Configuration data stream command.
    /// Used to stream configuration data in chunks.
    /// </summary>
    ConfigDataStream = 0x21,

    /// <summary>
    /// Data stream command for 6021 adapter.
    /// Used for communication with legacy 6021 control units.
    /// </summary>
    DataStream6021Adapter = 0x22,

    /// <summary>
    /// Automatic transmission command.
    /// Used for automation and scripted operations.
    /// </summary>
    AutomaticTransmission = 0x30,

    /// <summary>
    /// Debug message command (not in official specification).
    /// Used for diagnostic and debugging purposes.
    /// </summary>
    DebugMessage = 0x42,     // not is specification

    /// <summary>
    /// Unknown MS1 command 0x54.
    /// Purpose not documented in specification.
    /// </summary>
    MS1Unknown54 = 0x54,

    /// <summary>
    /// Unknown MS1 command 0x94.
    /// Purpose not documented in specification.
    /// </summary>
    MS1Unknown94 = 0x94
}
