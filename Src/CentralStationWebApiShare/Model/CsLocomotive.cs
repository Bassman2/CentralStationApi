namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsLocomotive 
{
    [CsProperty("funktionen")]
    public List<CsFunction>? Functions { get; private set; }

    [CsProperty("funktionen_2")]
    public List<CsFunction>? Functions2 { get; private set; }

    [CsProperty("name")]
    public string? Name { get; private set; }

    [CsProperty("uid")]
    public uint Uid { get; private set; }

    [CsProperty("mfxuid")]
    public uint MfxUid { get; private set; }

    [CsProperty("adresse")]
    public uint Adresse { get; private set; }

    [CsProperty("icon")]
    public string? Icon { get; private set; }

    [CsProperty("typ")]
    public string? Type { get; private set; }

    //
    [CsProperty("mfxtyp")]
    public uint MfxTyp { get; private set; }

    [CsProperty("blocks")]
    public uint[]? Blocks { get; private set; }
}
