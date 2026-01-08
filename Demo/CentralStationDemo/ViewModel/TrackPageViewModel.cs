namespace CentralStationDemo.ViewModel;

public partial class TrackPageViewModel : ObservableObject
{
    public TrackPageViewModel(Page page)
    {
        Id = page.Id;
        Name = page.Name;
    }

    [ObservableProperty]
    private uint id;

    [ObservableProperty]
    private string? name;
}
