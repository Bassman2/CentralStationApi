namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class RouteData
{
    [CsProperty("readonly")]
    public bool Readonly { get; private set; }

    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("fahrstrasse")]
    public List<Route>? Routes { get; private set; }
}
