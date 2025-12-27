namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Element
{
    [CsProperty("id")]
    public uint Id { get; private set; }

    [CsProperty("typ")]
    public string? Type { get; private set; }

    [CsProperty("drehung")]
    public int Rotation { get; private set; }

    [CsProperty("artikel")]
    public int Artikel { get; private set; }

    [CsProperty("text")]
    public string? Text { get; private set; }
}
