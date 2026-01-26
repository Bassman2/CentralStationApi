using System.IO;
using System.Windows.Media;
using System.ComponentModel;
using System.Reflection;

namespace CentralStationDemo.ViewModel;

public partial class ArticleViewModel : ObservableObject
{
    private readonly CentralStation cs;

    public ArticleViewModel(Article article, CentralStation cs)
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

    [ObservableProperty]
    private uint id;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private ArticleType articleType;

    [ObservableProperty]
    private string? articleTypeName;

    [ObservableProperty]
    private uint position;

    [ObservableProperty]
    private uint switchingTime;

    [ObservableProperty]
    private uint odd;

    [ObservableProperty]
    private DecoderType decoderType;

    [ObservableProperty]
    private Uri? iconUri;

}