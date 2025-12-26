namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class TrackDiagram
{
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("groesse")]
    public Size? Size { get; private set; }
}
