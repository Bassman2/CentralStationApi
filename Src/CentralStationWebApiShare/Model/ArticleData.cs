namespace CentralStationWebApi.Model;

[CsSerialize]
public partial class ArticleData
{
    [CsProperty("version")]
    public Version? Version { get; private set; }

    [CsProperty("artikel")]
    public List<Article>? Articles { get; private set; }
}
