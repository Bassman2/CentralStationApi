namespace CentralStationWebApi.Model;

/// <summary>
/// Represents a track layout element on a page in the Central Station.
/// Each element corresponds to a piece of track, accessory, or decoration placed on the layout.
/// </summary>
[CsSerialize]
public partial class Element
{
    /// <summary>
    /// Gets the unique identifier of the element within the layout.
    /// </summary>
    [CsProperty("id")]
    public uint Id { get; private set; }

    /// <summary>
    /// Gets the type of track element (e.g., "gerade" for straight track, "weiche" for turnout).
    /// Defines the visual representation and behavior of the element.
    /// </summary>
    [CsProperty("typ")]
    public string? Type { get; private set; }

    /// <summary>
    /// Gets the rotation angle of the element in degrees.
    /// Used to orient the element on the layout grid (e.g., 0, 90, 180, 270).
    /// </summary>
    [CsProperty("drehung")]
    public int Rotation { get; private set; }

    /// <summary>
    /// Gets the article ID reference linking this element to a specific article/accessory.
    /// References an <see cref="Article"/> that controls this track element.
    /// </summary>
    [CsProperty("artikel")]
    public int Artikel { get; private set; }

    /// <summary>
    /// Gets the text label or description associated with this element.
    /// Used for annotations or custom labels on the track layout.
    /// </summary>
    [CsProperty("text")]
    public string? Text { get; private set; }
}
