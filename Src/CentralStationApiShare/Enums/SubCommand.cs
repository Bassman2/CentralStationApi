namespace CentralStationApi;

/// <summary>
/// Specifies sub-command types used with <see cref="Command.SystemCommand"/> messages.
/// Sub-commands provide additional specificity for system-level operations on the Central Station.
/// </summary>
/// <remarks>
/// Sub-commands are only used with the <see cref="Command.SystemCommand"/> command type
/// and are encoded in byte 9 of the CAN message data payload.
/// </remarks>
public enum SubCommand : byte
{
    /// <summary>
    /// Stop command: Disables track power and stops all operations.
    /// All locomotives and accessories will cease operation.
    /// </summary>
    Stop = 0x00,

    /// <summary>
    /// Go command: Enables track power and resumes normal operations.
    /// Locomotives and accessories can be controlled after this command.
    /// </summary>
    Go = 0x01,

    /// <summary>
    /// Halt command: Emergency stop for the entire system.
    /// Similar to Stop but typically used for emergency situations.
    /// </summary>
    Halt = 0x02,

    /// <summary>
    /// Locomotive emergency halt command: Stops all locomotives immediately.
    /// Accessories continue to operate; only locomotive movement is halted.
    /// </summary>
    LocoHalt = 0x03,

    /// <summary>
    /// Locomotive cycle stop command: Stops the locomotive programming cycle.
    /// Used during decoder programming or registration operations.
    /// </summary>
    LocoCycleStop = 0x04,

    /// <summary>
    /// Locomotive data protocol command: Sets or queries the locomotive decoder protocol.
    /// Used to configure MM2, mfx, or DCC protocol settings.
    /// </summary>
    LocoDataProtocol = 0x05,

    /// <summary>
    /// Switching time command: Sets or queries the switching time for accessories.
    /// Defines how long power is applied to turnout motors and other accessories.
    /// </summary>
    SwitchingTime = 0x06,

    /// <summary>
    /// Fast read command: Initiates fast CV (configuration variable) reading.
    /// Used for rapid decoder configuration value reading.
    /// </summary>
    FastRead = 0x07,

    /// <summary>
    /// Track protocol command: Sets or queries the active track protocol.
    /// Controls which decoder protocols (MM2, mfx, DCC) are enabled on the track.
    /// </summary>
    TrackProtocol = 0x08,

    /// <summary>
    /// New registration counter command: Sets or queries the mfx new registration counter.
    /// Controls how many new mfx decoders can be automatically registered.
    /// </summary>
    NewRegistrationCounter = 0x09,

    /// <summary>
    /// Overload notification: Reports track overload or short circuit conditions.
    /// Sent by boosters (TFP/GFP) when excessive current is detected. Response only.
    /// </summary>
    Overload = 0x0A,

    /// <summary>
    /// Status query command: Requests or reports system status information.
    /// Used to query configuration values and operational parameters from devices.
    /// </summary>
    Status = 0x0B,

    /// <summary>
    /// Identifier command: Sets or queries device identifier information.
    /// Used for device identification and configuration.
    /// </summary>
    Identifier = 0x0C,

    /// <summary>
    /// Unknown sub-command 0x20 (not in official specification).
    /// Purpose and usage are undocumented.
    /// </summary>
    Unknown20 = 0x20,

    /// <summary>
    /// mfx seek command: Initiates mfx decoder search and registration.
    /// Used to discover and register new mfx locomotives on the track.
    /// </summary>
    MfxSeek = 0x30,

    /// <summary>
    /// Reset command: Resets a device or system component to default state.
    /// Can reset specific devices or system-wide settings.
    /// </summary>
    Reset = 0x80,
}
