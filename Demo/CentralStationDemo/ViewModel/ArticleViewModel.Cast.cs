namespace CentralStationDemo.ViewModel;


partial class ArticleViewModel
{

    protected readonly CentralStation centralStation;

    public ArticleViewModel(ArticleModel mode, CentralStation centralStation)
    {
        this.centralStation = centralStation;

        this.Id = mode.Id;
        this.Name = mode.Name;
        this.ArticleType = mode.ArticleType;
        this.ArticleTypeName = mode.ArticleTypeName;
        this.Position = mode.Position;
        this.SwitchingTime = mode.SwitchingTime;
        this.Odd = mode.Odd;
        this.DecoderType = mode.DecoderType;
        this.IconUri = mode.IconUri;
    }


    public static List<ArticleViewModel> FromModels(IEnumerable<ArticleModel>? models, CentralStation centralStation)
        => models?.Select(model => new ArticleViewModel(model, centralStation)).ToList() ?? [];


}

static class ArticleViewModelStaticCast
{

    public static List<ArticleViewModel> FromModels(this IEnumerable<ArticleModel>? models, CentralStation centralStation)
        => models?.Select(model => new ArticleViewModel(model, centralStation)).ToList() ?? [];

}

