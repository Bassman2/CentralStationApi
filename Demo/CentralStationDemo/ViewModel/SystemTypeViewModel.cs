using CentralStationDemo.Enums;

namespace CentralStationDemo.ViewModel;

public partial class SystemTypeViewModel(SystemTypes systemTypes) : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string icom;
}
