namespace CentralStationDemo.ViewModel;


public partial class ArticleViewModel
{
    private readonly CentralStation cs;

    public ArticleViewModel(ArticleModel article, CentralStation cs)
    {
        this.cs = cs;

        Id = article.Id;
        Name = article.Name;
        ArticleType = article.ArticleType;
        ArticleTypeName = article.ArticleTypeName;
        //ArticleTypeName = typeof(ArticleType).GetField(article.ArticleType.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        Position = article.Position;
        SwitchingTime = article.SwitchingTime;
        Odd = article.Odd;
        DecoderType = article.DecoderType;
        IconUri = article.IconUri;
    }

    //public static implicit operator ArticleViewModel(ArticleModel article) => new(article, cs);

    public static ArticleViewModel? FromArticle(ArticleModel? article, CentralStation cs)
        => article is null ? null : new ArticleViewModel(article, cs);

    public static List<ArticleViewModel> FromArticles(IEnumerable<ArticleModel>? articles, CentralStation cs)
        => [.. articles?.Select(a => new ArticleViewModel(a, cs)) ?? []];
}

public static class ArticleViewModelStaticConverter
{
    public static List<ArticleViewModel> FromArticles(this IEnumerable<ArticleModel>? articles, CentralStation cs)
        => [.. articles?.Select(a => new ArticleViewModel(a, cs)) ?? []];
}
