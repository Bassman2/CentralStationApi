namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class RailwayRoutes
{
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("artikel")]
    public List<RailwayRoute>? RailwayRoutes_ { get; private set; }
}
