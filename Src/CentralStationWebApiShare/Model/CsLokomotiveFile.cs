namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsLokomotiveFile 
{   
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("lokomotive")]
    public List<CsLocomotive>? Locomotives { get; private set; }
}
