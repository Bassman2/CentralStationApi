namespace CentralStationApi.Model;

/// <summary>
/// Represents track layout data from the Central Station.
/// Contains information about the track configuration including pages, size, and version.
/// </summary>
[CsSerialize]
public partial class TrackData
{
    /// <summary>
    /// Gets a value indicating whether the track data is read-only.
    /// When true, the track layout cannot be modified.
    /// </summary>
    [CsProperty("readonly")]
    public bool Readonly { get; private set; }

    /// <summary>
    /// Gets the version information of the track data.
    /// </summary>
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    /// <summary>
    /// Gets the size dimensions of the track layout.
    /// </summary>
    [CsProperty("groesse")]
    public Size? Size { get; private set; }

    /// <summary>
    /// Gets the list of pages containing the track layout elements.
    /// Each page represents a section or view of the overall track configuration.
    /// </summary>
    [CsProperty("seite")]
    public List<Page>? Pages { get; private set; }
}
