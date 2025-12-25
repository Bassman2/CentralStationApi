namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class CsMagneticItems
{
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("artikel")]
    public List<CsArticle>? Articles { get; private set; }
}
