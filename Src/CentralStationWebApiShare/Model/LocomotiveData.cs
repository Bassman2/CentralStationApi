namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class LocomotiveData 
{   
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("lokomotive")]
    public List<Locomotive>? Locomotives { get; private set; }
}
