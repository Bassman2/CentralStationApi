namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Size
{
    [CsProperty("width")]
    public uint Width { get; private set; }

    [CsProperty("height")]
    public uint Height { get; private set; }
}
