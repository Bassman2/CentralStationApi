namespace CentralStationDemo.ViewModel;

//[AutoConverterClass(nameof(ArticleModel), AdditionalParameter = nameof(CentralStation))]
public partial class ArticleViewModel : ObservableObject
{
    //private readonly CentralStation cs;

    //public ArticleViewModel(Article article, CentralStation cs)
    //{
    //    this.cs = cs;

    //    Id = article.Id;
    //    Name = article.Name;
    //    ArticleType = article.ArticleType;
    //    ArticleTypeName = article.ArticleTypeName; 
    //    //ArticleTypeName = typeof(ArticleType).GetField(article.ArticleType.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    //    Position = article.Position;
    //    SwitchingTime = article.SwitchingTime;
    //    Odd = article.Odd;
    //    DecoderType = article.DecoderType;
    //    IconUri = article.IconUri;
    //}

    //public ArticleViewModel(CentralStation cs, ArticleModel article)
    //{
    //    this.cs = cs;

    //    Id = article.Id;
    //    Name = article.Name;
    //    ArticleType = article.ArticleType;
    //    ArticleTypeName = article.ArticleTypeName;
    //    //ArticleTypeName = typeof(ArticleType).GetField(article.ArticleType.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    //    Position = article.Position;
    //    SwitchingTime = article.SwitchingTime;
    //    Odd = article.Odd;
    //    DecoderType = article.DecoderType;
    //    IconUri = article.IconUri;
    //}

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Id))]
    public partial uint Id { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Name))]
    public partial string? Name { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.ArticleType))]
    public partial ArticleType ArticleType { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.ArticleTypeName))]
    public partial string? ArticleTypeName { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Position))]
    public partial uint Position { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.SwitchingTime))]
    public partial uint SwitchingTime { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Odd))]
    public partial uint Odd { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.DecoderType))]
    public partial DecoderType DecoderType { get; set; }

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.ImagePath))]
    public partial string? ImagePath { get; set; }
}