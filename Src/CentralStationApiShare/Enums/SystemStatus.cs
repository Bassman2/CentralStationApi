namespace CentralStationApi;

/// <summary>
/// Represents the current operational status of the Central Station system.
/// Indicates whether track power is enabled and locomotives/accessories can operate.
/// </summary>
/// <remarks>
/// The system status is controlled by sending <see cref="SubCommand.Stop"/> or 
/// <see cref="SubCommand.Go"/> commands and affects all track operations.
/// This status is exposed through the <see cref="CentralStation.Status"/> property.
/// </remarks>
public enum SystemStatus
{
    /// <summary>
    /// System is stopped: Track power is disabled.
    /// Locomotives and accessories cannot operate. This is the safe state for track maintenance
    /// or when no operations should occur.
    /// </summary>
    Stop,

    /// <summary>
    /// System is running: Track power is enabled.
    /// Locomotives and accessories can be controlled and operated normally.
    /// </summary>
    Go,

    /// <summary>
    /// Default or unknown system status.
    /// Represents the initial state before the first status update is received.
    /// </summary>
    Default
}
