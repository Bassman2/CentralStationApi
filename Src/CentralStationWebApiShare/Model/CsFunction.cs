namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsFunction 
{
    [CsProperty("nr")]
    public int Num { get; private set; }

    [CsProperty("typ")]
    public int Type { get; private set; }

    [CsProperty("wert")]
    public int Value { get; private set; }
}
