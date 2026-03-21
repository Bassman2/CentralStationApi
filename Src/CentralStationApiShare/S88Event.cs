namespace CentralStationApi;

/// <summary>
/// Represents an S88 feedback bus event from the Central Station.
/// Contains information about a state change on an S88 contact, including the old and new values and timing.
/// </summary>
/// <remarks>
/// S88 is a feedback bus system used in model railways to detect train positions and track occupancy.
/// Events are triggered when contacts change state (e.g., a train enters or leaves a detection section).
/// </remarks>
public class S88Event
{
    /// <summary>
    /// Gets or sets the previous state of the S88 contact before the event.
    /// </summary>
    /// <value>
    /// Typically 0 for inactive/unoccupied or 1 for active/occupied, depending on the contact type.
    /// </value>
    public byte OldValue { get; internal set; }

    /// <summary>
    /// Gets or sets the new state of the S88 contact after the event.
    /// </summary>
    /// <value>
    /// Typically 0 for inactive/unoccupied or 1 for active/occupied, depending on the contact type.
    /// </value>
    public byte NewValue { get; internal set; }

    /// <summary>
    /// Gets or sets the timestamp or duration associated with the event.
    /// </summary>
    /// <value>
    /// Time value in milliseconds, representing either the event timestamp or the duration
    /// the contact has been in the current state.
    /// </value>
    public ushort Time { get; internal set; }
}
