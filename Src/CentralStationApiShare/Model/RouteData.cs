namespace CentralStationApi.Model;

/// <summary>
/// Represents the collection of route data from the Central Station.
/// Contains version information and a list of all routes (Fahrstrassen) configured in the system.
/// </summary>
/// <remarks>
/// Routes (Fahrstrassen in German) are automated sequences of turnout and signal operations
/// that can be activated with a single command to set up paths for train movements.
/// This class serves as the root container when deserializing route configuration files.
/// </remarks>
[CsSerialize]
public partial class RouteData
{
    /// <summary>
    /// Gets a value indicating whether the route data is read-only.
    /// When true, the route configuration cannot be modified.
    /// </summary>
    [CsProperty("readonly")]
    public bool Readonly { get; private set; }

    /// <summary>
    /// Gets the version information of the route data.
    /// </summary>
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    /// <summary>
    /// Gets the list of routes (Fahrstrassen) configured in the Central Station.
    /// Each <see cref="Route"/> defines an automated sequence of accessory commands.
    /// </summary>
    [CsProperty("fahrstrasse")]
    public List<Route>? Routes { get; private set; }
}
