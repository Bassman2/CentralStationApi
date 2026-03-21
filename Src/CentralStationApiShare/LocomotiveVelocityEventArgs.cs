namespace CentralStationApi;

/// <summary>
/// Provides data for locomotive velocity (speed) change events.
/// Contains the locomotive identifier and the new velocity value.
/// </summary>
/// <remarks>
/// This event is raised when a locomotive's speed is changed via the Central Station,
/// typically in response to speed commands from controllers, throttles, or automation sequences.
/// The velocity value ranges from <see cref="CentralStation.MinVelocity"/> (0 = stopped) to
/// <see cref="CentralStation.MaxVelocity"/> (1000 = maximum speed).
/// The event is triggered by <see cref="Command.LocoVelocity"/> responses on the CAN bus.
/// </remarks>
public class LocomotiveVelocityEventArgs(uint locomotiveId, ushort velocity) : LocomotiveEventArgs(locomotiveId)
{
    /// <summary>
    /// Gets the velocity (speed) of the locomotive.
    /// </summary>
    /// <value>
    /// The velocity value ranging from 0 (stopped) to 1000 (maximum speed).
    /// The actual top speed depends on the locomotive's decoder configuration andCV settings.
    /// </value>
    public ushort Velocity => velocity;
}
