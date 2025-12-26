namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Locomotives 
{   
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("lokomotive")]
    public List<Locomotive>? Locomotives_ { get; private set; }
}
