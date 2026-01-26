
namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class RouteItem
{
    [CsProperty("typ")]
    public RouteItemType Type { get; private set; }

    [CsProperty("sekunde")]
    public int Sekunde { get; private set; }

    /// <summary>
    /// Reference to another route. Either the route number or -1 for magnetic articles.
    /// When referring to another route, the magnetic article and position must be set to 0.
    /// </summary>
    [CsProperty("fsverweis")]
    public int Reference { get; private set; }

    /// <summary>
    /// Magnetic article, number from keyboard, 0 when referring to further route.
    /// </summary>
    [CsProperty("magnetartikel")]
    public uint Article { get; private set; }

    /// <summary>
    /// Position to be switched, depending on the article, between 1 and 4 or 0 when referring to another route.
    /// </summary>
    [CsProperty("stellung")]
    public uint Position { get; private set; }

}
