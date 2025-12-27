namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Page
{
    [CsProperty("id")]
    public uint Id { get; private set; }

    [CsProperty("name")]
    public string? Name { get; private set; }

}

