namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsRailwayRoutes
{
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("artikel")]
    public List<CsRailwayRoute>? RailwayRoutes { get; private set; }
}
