namespace CentralStationApi.Model;

/// <summary>
/// Represents the dimensions of a track layout in the Central Station.
/// Defines the width and height of the layout grid in track element units.
/// </summary>
/// <remarks>
/// The size defines the dimensions of the track layout canvas where <see cref="Element"/>
/// instances are placed. The width and height are typically measured in grid units
/// corresponding to standard track piece dimensions.
/// </remarks>
[CsSerialize]
public partial class Size
{
    /// <summary>
    /// Gets the width of the track layout.
    /// </summary>
    /// <value>
    /// The width in grid units, representing the horizontal dimension of the layout canvas.
    /// </value>
    [CsProperty("width")]
    public uint Width { get; private set; }

    /// <summary>
    /// Gets the height of the track layout.
    /// </summary>
    /// <value>
    /// The height in grid units, representing the vertical dimension of the layout canvas.
    /// </value>
    [CsProperty("height")]
    public uint Height { get; private set; }
}
