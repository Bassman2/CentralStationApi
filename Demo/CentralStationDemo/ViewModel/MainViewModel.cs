using CommunityToolkit.Mvvm.Input;

namespace CentralStationDemo.ViewModel;

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private CentralStation cs; 
    public MainViewModel()
    {
        cs = new CentralStation();
        cs.MessageReceived += (s, e) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Messages.Insert(0, new MessageViewModel(e.Message));
            });
        };
    }

    public void Dispose()
    {
        //cs.Close();
        cs.Dispose();
    }

    [ObservableProperty]
    private ObservableCollection<MessageViewModel> messages = [];

    [RelayCommand]
    private async Task OnSystemStop()
    { 
        await cs.SystemStopAsync();
    }


    [RelayCommand]
    private async Task OnSystemGo()
    {
        await cs.SystemGoAsync();
    }

    [RelayCommand]
    private async Task OnSystemHalt()
    {
        await cs.SystemHaltAsync();
    }

    [RelayCommand]
    private async Task OnLocoInfo()
    {
        await cs.ConfigDataLocoInfo();
    }

    [RelayCommand]
    private async Task OnLocos()
    {
        string file = await cs.ConfigDataLocos();
    }

    
}
