namespace CentralStationApi.Model;

/// <summary>
/// Represents a route (Fahrstrasse) in the Central Station.
/// Routes define automated sequences of turnout and signal operations to set up paths for train movements.
/// </summary>
/// <remarks>
/// A route is a predefined sequence of accessory commands (turnouts, signals, etc.) that can be
/// activated with a single command. Routes may be triggered manually, by S88 feedback events,
/// or externally from other systems.
/// </remarks>
[CsSerialize]
public partial class Route
{
    /// <summary>
    /// Gets the unique identifier of the route.
    /// </summary>
    [CsProperty("id")]
    public uint Id { get; private set; }

    /// <summary>
    /// Gets the name or description of the route.
    /// Used to identify the route in the Central Station interface.
    /// </summary>
    [CsProperty("name")]
    public string? Name { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the route uses S88 feedback for activation or monitoring.
    /// When true, the route responds to S88 feedback bus events.
    /// </summary>
    [CsProperty("s88")]
    public bool S88 { get; private set; }

    /// <summary>
    /// Gets a value indicating the S88 activation state for the route.
    /// Defines whether the route is triggered when an S88 contact is activated (on) or deactivated (off).
    /// </summary>
    [CsProperty("s88Ein")]
    public bool S88On { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the route can be triggered externally.
    /// When true, the route can be activated by external systems or protocols.
    /// </summary>
    [CsProperty("extern")]
    public bool Extern { get; private set; }

    /// <summary>
    /// Gets the list of route items that define the sequence of operations.
    /// Each <see cref="RouteItem"/> represents an individual accessory command (turnout position, signal aspect, etc.).
    /// </summary>
    [CsProperty("item")]
    public List<RouteItem>? Items { get; private set; }
}
