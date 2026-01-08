namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Route
{

    [CsProperty("id")]
    public uint Id { get; private set; }

    [CsProperty("name")]
    public string? Name { get; private set; }

    [CsProperty("s88")]
    public bool S88 { get; private set; }

    [CsProperty("s88Ein")]
    public bool S88On { get; private set; }

    [CsProperty("extern")]
    public bool Extern { get; private set; }

    [CsProperty("item")]
    public List<RouteItem>? Items { get; private set; }
}
