namespace CentralStationApi.Model;

/// <summary>
/// Represents a page in the Central Station track layout.
/// Pages are used to organize track elements into separate views or sections of the overall layout.
/// </summary>
/// <remarks>
/// Each page can contain multiple <see cref="Element"/> instances that define the track pieces,
/// accessories, and decorations placed on that particular page/view.
/// </remarks>
[CsSerialize]
public partial class Page
{
    /// <summary>
    /// Gets the unique identifier of the page within the track layout.
    /// </summary>
    [CsProperty("id")]
    public uint Id { get; private set; }

    /// <summary>
    /// Gets the name or title of the page.
    /// Used to identify and distinguish between different layout sections or views.
    /// </summary>
    [CsProperty("name")]
    public string? Name { get; private set; }

}

