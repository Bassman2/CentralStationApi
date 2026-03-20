namespace CentralStationWebApi;

/// <summary>
/// Provides data for locomotive-related events.
/// Contains the identifier of the locomotive that triggered the event.
/// </summary>
public class LocomotiveEventArgs(uint locomotiveId) : EventArgs
{
    /// <summary>
    /// Gets the unique identifier of the locomotive associated with this event.
    /// </summary>
    /// <value>
    /// The locomotive ID (UID) that identifies the specific locomotive in the Central Station system.
    /// </value>
    public uint LocomotiveId => locomotiveId;
}
