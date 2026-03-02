//namespace CentralStationDemo.Model;

//public partial class ArticleModel
//{
//    public ArticleModel()
//    { }

//    public ArticleModel(Article article)
//    { 
//        Id = article.Id;
//        Name = article.Name;
//        ArticleType = article.ArticleType;
//        ArticleTypeName = article.ArticleTypeName;
//        Position = article.Position;
//        SwitchingTime = article.SwitchingTime;
//        Odd = article.Odd;
//        DecoderType = article.DecoderType;
//        IconUri = article.IconUri;
//    }

//    //public void ApplyChangesTo(Article article)
//    //{
//    //    ArgumentNullException.ThrowIfNull(article);

//    //    article.Id = Id;
//    //    article.Name = Name;
//    //    article.ArticleType = ArticleType;
//    //    article.ArticleTypeName = ArticleTypeName;
//    //    article.Position = Position;
//    //    article.SwitchingTime = SwitchingTime;
//    //    article.Odd = Odd;
//    //    article.DecoderType = DecoderType;
//    //    article.IconUri = IconUri;
//    //}   

//    public static implicit operator ArticleModel(Article article) => new(article);

//    public static List<ArticleModel> FromArticles(IEnumerable<Article>? articles)
//        => [.. articles?.Select(a => new ArticleModel(a)) ?? []];

//    //public static List<ArticleModel> FromArticles(IEnumerable<Article> articles)
//    //    => articles.Select(a => new ArticleModel(a)).ToList();

//    //public static implicit operator Article(ArticleModel articleModel) 
//    //    => new Article(articleModel.Id, articleModel.Name, articleModel.ArticleType, articleModel.ArticleTypeName, articleModel.Position, articleModel.SwitchingTime, articleModel.Odd, articleModel.DecoderType, articleModel.IconUri);

//}

//public static class ArticleModelStaticConverter
//{
//    public static List<ArticleModel> FromArticles(this IEnumerable<Article>? articles)
//        => [.. articles?.Select(a => new ArticleModel(a)) ?? []];

//}
