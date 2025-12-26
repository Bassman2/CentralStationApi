namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class MagneticItems
{
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("artikel")]
    public List<Article>? Articles { get; private set; }
}
