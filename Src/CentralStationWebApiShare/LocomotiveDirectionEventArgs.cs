namespace CentralStationWebApi;

/// <summary>
/// Provides data for locomotive direction change events.
/// Contains the locomotive identifier and the new direction of travel.
/// </summary>
/// <remarks>
/// This event is raised when a locomotive's direction is changed via the Central Station,
/// typically in response to direction commands sent by controllers or automation sequences.
/// The event is triggered by <see cref="Command.LocoDirection"/> responses on the CAN bus.
/// </remarks>
public class LocomotiveDirectionEventArgs(uint locomotiveId, DirectionChange direction) : LocomotiveEventArgs(locomotiveId)
{
    /// <summary>
    /// Gets the direction change command or state for the locomotive.
    /// </summary>
    /// <value>
    /// The <see cref="DirectionChange"/> value indicating the new direction (Forward, Backward, Toggle, or Remain).
    /// </value>
    public DirectionChange Direction => direction;
}
