namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class ArticleData
{
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    [CsProperty("artikel")]
    public List<Article>? Articles { get; private set; }
}
