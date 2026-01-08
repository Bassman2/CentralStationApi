namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class TrackPageData
{
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("page")]
    public uint Page { get; private set; }

    [CsProperty("element")]
    public List<Element>? Elements { get; private set; }
}

