using System.IO;
using System.Windows.Media;

namespace CentralStationDemo.ViewModel;

public partial class ArticleViewModel : ObservableObject
{
    private readonly CentralStation cs;

    public ArticleViewModel(Article article, CentralStation cs)
    {
        this.cs = cs;

        Id = article.Id;
        Name = article.Name;
        Type = article.Type;
        Position = article.Position;
        SwitchingTime = article.SwitchingTime;
        Odd = article.Odd;
        DecoderType = article.DecoderType;
        IconUri = article.IconUri;

        //using var file = File.OpenRead("Images/magicon_a_005_01.svg");
        //image = SvgConverter.ConvertSvg(file);
    }

    [ObservableProperty]
    private uint id;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private ArticleType type;

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

    //[ObservableProperty]
    //private ImageSource image;
}