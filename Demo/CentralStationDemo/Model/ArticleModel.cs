namespace CentralStationDemo.Model;

[AutoConverterClass(nameof(Article))]
public partial class ArticleModel
{
    public ArticleModel() { }

    [JsonPropertyName("id")]
    [AutoConverterProperty(nameof(Article.Id))]
    public uint Id { get; set; }

    [JsonPropertyName("name")]
    [AutoConverterProperty(nameof(Article.Name))]
    public string? Name { get; set; }

    [JsonPropertyName("articleType")]
    [AutoConverterProperty(nameof(Article.ArticleType))]
    public ArticleType ArticleType { get; set; }

    [JsonPropertyName("articleTypeName")]
    [AutoConverterProperty(nameof(Article.ArticleTypeName))]
    public string? ArticleTypeName { get; set; }

    [JsonPropertyName("position")]
    [AutoConverterProperty(nameof(Article.Position))]
    public uint Position { get; set; }

    [JsonPropertyName("switchingTime")]
    [AutoConverterProperty(nameof(Article.SwitchingTime))]
    public uint SwitchingTime { get; set; }

    [JsonPropertyName("odd")]
    [AutoConverterProperty(nameof(Article.Odd))]
    public uint Odd { get; set; }

    [JsonPropertyName("decoderType")]
    [AutoConverterProperty(nameof(Article.DecoderType))]
    public DecoderType DecoderType { get; set; }

    [JsonPropertyName("iconUri")]
    [AutoConverterProperty(nameof(Article.IconUri))]
    public Uri? IconUri { get; set; }
}
