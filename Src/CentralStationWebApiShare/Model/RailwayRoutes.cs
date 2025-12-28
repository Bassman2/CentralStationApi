namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class RailwayRoutes
{
    [CsProperty("readonly")]
    public bool Readonly { get; private set; }

    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("artikel")]
    public List<RailwayRoute>? RailwayRoutes_ { get; private set; }
}
