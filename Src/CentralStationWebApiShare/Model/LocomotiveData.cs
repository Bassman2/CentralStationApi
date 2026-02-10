namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class LocomotiveData 
{   
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("lokomotive")]
    public List<Locomotive>? Locomotives { get; private set; }
}
