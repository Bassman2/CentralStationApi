namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class TrackData
{
    [CsProperty("readonly")]
    public bool Readonly { get; private set; }

    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("groesse")]
    public Size? Size { get; private set; }

    [CsProperty("seite")]
    public List<Page>? Pages { get; private set; }
}
