namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class Version 
{
    [CsProperty("major")]
    public int Major { get; private set; } = 0;

    [CsProperty("minor")]
    public int Minor { get; private set; } = 0;

}

