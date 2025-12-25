namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsTrackDiagram
{
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("groesse")]
    public CsSize? Size { get; private set; }
}
