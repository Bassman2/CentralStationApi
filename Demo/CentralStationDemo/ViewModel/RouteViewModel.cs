namespace CentralStationDemo.ViewModel;

public partial class RouteViewModel : ObservableObject
{
    public RouteViewModel(Route route)
    {
        Id = route.Id;
        Name = route.Name;
        S88 = route.S88;
        S88On = route.S88On;
        IsExtern = route.Extern;
        Items = route.Items;
    }

    [ObservableProperty]
    private uint id;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private bool s88;

    [ObservableProperty]
    private bool s88On;

    [ObservableProperty]
    private bool isExtern;

    [ObservableProperty]
    private List<RouteItem>? items;
}
