namespace CentralStationDemo.ViewModel;

public partial class MagneticItemViewModel : ObservableObject
{
    public MagneticItemViewModel(Article article)
    {
        Id = article.Id;
        Name = article.Name;
        Type = article.Type;
        Position = article.Position;
        SwitchingTime = article.SwitchingTime;
        Odd = article.Odd;
        DecoderType = article.DecoderType;
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
}