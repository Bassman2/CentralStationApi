using CentralStationDemo.Enums;

namespace CentralStationDemo.ViewModel;

public partial class SystemTypeViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? Icom { get; set; }
}
