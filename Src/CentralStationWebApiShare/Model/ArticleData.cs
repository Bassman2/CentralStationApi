namespace CentralStationWebApi.Model;

/// <summary>
/// Represents a collection of article data from the Central Station.
/// Contains version information and a list of articles (devices/accessories).
/// </summary>
[CsSerialize]
public partial class ArticleData
{
    /// <summary>
    /// Gets the version information of the article data.
    /// </summary>
    [CsProperty("version")]
    public CsVersion? Version { get; private set; }

    /// <summary>
    /// Gets the list of articles (locomotives, switches, signals, etc.) configured in the Central Station.
    /// </summary>
    [CsProperty("artikel")]
    public List<Article>? Articles { get; private set; }
}
