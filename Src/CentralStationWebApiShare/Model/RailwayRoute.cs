namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class RailwayRoute
{

    [CsProperty("id")]
    public uint Uid { get; private set; }

    [CsProperty("Id")]
    public string? Name { get; private set; }
}
