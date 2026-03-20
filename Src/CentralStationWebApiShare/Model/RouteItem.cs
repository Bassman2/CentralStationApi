namespace CentralStationWebApi.Model;

/// <summary>
/// Represents a single item/step in a route sequence.
/// Each item defines either an accessory command or a reference to another route.
/// </summary>
/// <remarks>
/// A route item can be one of two types:
/// <list type="bullet">
/// <item><description>A magnetic article command: Sets a specific position for a turnout, signal, or other accessory</description></item>
/// <item><description>A route reference: Triggers another route as part of the sequence</description></item>
/// </list>
/// The <see cref="Reference"/> property determines which type: -1 for article commands, or a route ID for route references.
/// </remarks>
[CsSerialize]
public partial class RouteItem
{
    /// <summary>
    /// Gets the type of route item.
    /// </summary>
    /// <value>
    /// The <see cref="RouteItemType"/> indicating the category of this item (typically <see cref="RouteItemType.Mag"/> for magnetic accessories).
    /// </value>
    [CsProperty("typ")]
    public RouteItemType Type { get; private set; }

    /// <summary>
    /// Gets the delay time in seconds before executing this item.
    /// </summary>
    /// <value>
    /// The number of seconds to wait before executing this route item.
    /// Used to create timed sequences where items are executed sequentially with delays.
    /// </value>
    [CsProperty("sekunde")]
    public int Sekunde { get; private set; }

    /// <summary>
    /// Gets the reference to another route, or -1 for magnetic article commands.
    /// </summary>
    /// <value>
    /// <list type="bullet">
    /// <item><description>-1: This item controls a magnetic article (see <see cref="Article"/> and <see cref="Position"/>)</description></item>
    /// <item><description>Route ID (≥0): This item triggers the specified route. In this case, <see cref="Article"/> and <see cref="Position"/> must be 0.</description></item>
    /// </list>
    /// </value>
    /// <remarks>
    /// When referring to another route, the magnetic article and position must be set to 0.
    /// This allows routes to call other routes, enabling complex nested automation sequences.
    /// </remarks>
    [CsProperty("fsverweis")]
    public int Reference { get; private set; }

    /// <summary>
    /// Gets the magnetic article address/number from the keyboard.
    /// </summary>
    /// <value>
    /// The article number to control. Must be 0 when <see cref="Reference"/> points to another route.
    /// For magnetic articles, this corresponds to the article address configured in the Central Station.
    /// </value>
    /// <remarks>
    /// Magnetic article, number from keyboard, 0 when referring to further route.
    /// </remarks>
    [CsProperty("magnetartikel")]
    public uint Article { get; private set; }

    /// <summary>
    /// Gets the position/state to switch the article to.
    /// </summary>
    /// <value>
    /// The position value, typically between 1 and 4 depending on the article type:
    /// <list type="bullet">
    /// <item><description>0: When <see cref="Reference"/> points to another route</description></item>
    /// <item><description>1-2: For two-position accessories (turnouts, signals with two aspects)</description></item>
    /// <item><description>1-3: For three-position accessories (three-way turnouts, signals with three aspects)</description></item>
    /// <item><description>1-4: For four-position accessories (double slip switches, signals with four aspects)</description></item>
    /// </list>
    /// </value>
    /// <remarks>
    /// Position to be switched, depending on the article, between 1 and 4 or 0 when referring to another route.
    /// The valid range depends on the capabilities of the specific article being controlled.
    /// </remarks>
    [CsProperty("stellung")]
    public uint Position { get; private set; }

}
