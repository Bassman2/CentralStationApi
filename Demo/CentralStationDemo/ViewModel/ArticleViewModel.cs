namespace CentralStationDemo.ViewModel;

[AutoConverterClass(nameof(ArticleModel), AdditionalParameter = nameof(CentralStation))]
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
    private uint id;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Name))]
    private string? name;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.ArticleType))]
    private ArticleType articleType;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.ArticleTypeName))]
    private string? articleTypeName;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Position))]
    private uint position;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.SwitchingTime))]
    private uint switchingTime;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.Odd))]
    private uint odd;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.DecoderType))]
    private DecoderType decoderType;

    [ObservableProperty]
    [AutoConverterProperty(nameof(ArticleModel.IconUri))]
    private Uri? iconUri;
}