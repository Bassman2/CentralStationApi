namespace CentralStationApi.Model;

/// <summary>
/// Represents the data for a single track layout page from the Central Station.
/// Contains version information, page identifier, and the elements placed on that page.
/// </summary>
/// <remarks>
/// Track layouts are organized into multiple pages, where each page represents a section or view
/// of the overall layout. This class is used when deserializing individual page files from the
/// Central Station configuration, as opposed to <see cref="TrackData"/> which contains all pages.
/// Each element on the page represents a track piece, accessory, or decoration.
/// </remarks>
[CsSerialize]
public partial class TrackPageData
{
    /// <summary>
    /// Gets the version information of the track page data.
    /// </summary>
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    /// <summary>
    /// Gets the page identifier/number.
    /// </summary>
    /// <value>
    /// The unique identifier or sequential number of this page within the track layout.
    /// </value>
    [CsProperty("page")]
    public uint Page { get; private set; }

    /// <summary>
    /// Gets the list of elements placed on this track layout page.
    /// Each <see cref="Element"/> represents a track piece, turnout, signal, or other item
    /// positioned on the layout grid.
    /// </summary>
    [CsProperty("element")]
    public List<Element>? Elements { get; private set; }
}

