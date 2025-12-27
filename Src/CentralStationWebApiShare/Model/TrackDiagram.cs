namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class TrackDiagram
{
    [CsProperty("readonly")]
    public bool Readonly { get; private set; }

    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("groesse")]
    public Size? Size { get; private set; }

    [CsProperty("seite")]
    public List<Page>? Pages { get; private set; }
}
