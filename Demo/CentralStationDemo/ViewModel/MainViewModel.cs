using CentralStationWebApi.Model;

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

                UpdateStatus(e.Message);
            });
        };
        cs.FileReceived += (s, e) =>
        {
            ScanStreams(e.CSFileStream); 
        };
    }

    public void Dispose()
    {
        //cs.Close();
        cs.Dispose();
    }

    protected override void OnStartup()
    {
        cs.RequestConfigDataLocos();
    }

    private void UpdateStatus(CANMessage message)
    { 
        if (message.Command == Command.SystemCommand && message.SystemCommand == SystemCommand.Stop && message.Device == CentralStation.AllDevices)
        {
            SystemStatus = SystemStatus.STOP;
        }
        if (message.Command == Command.SystemCommand && message.SystemCommand == SystemCommand.Go && message.Device == CentralStation.AllDevices)
        {
            SystemStatus = SystemStatus.GO;
        }
    }

    private void ScanStreams(CSFileStream cSFileStream)
    {
        var data = CsSerializer.Deserialize<CsLokomotiveFile>(cSFileStream.GetFileStream(), "[lokomotive]");
    }

    [ObservableProperty]
    private SystemStatus systemStatus = SystemStatus.STOP;

    [ObservableProperty]
    private uint device = 0;

    //partial void OnDeviceChanged(uint value)
    //{ 
    //}

    [ObservableProperty]
    private ObservableCollection<MessageViewModel> messages = [];

    #region System Sync Commands

    [RelayCommand]
    private void OnSystemStop() => cs.SystemStop();
    
    [RelayCommand]
    private void OnSystemGo() => cs.SystemGo();
    
    [RelayCommand]
    private void OnSystemHalt() => cs.SystemHalt();

    [RelayCommand]
    private void OnSystemLocoHalt() => cs.SystemLocoHalt();

    [RelayCommand]
    private void OnSystemLocoCycleStop() => cs.SystemLocoCycleStop();


    #endregion

    #region System Async Commands

    [RelayCommand]
    private async Task OnSystemStopAsyncAsync()
    {
        await cs.SystemStopAsync();
    }


    [RelayCommand]
    private async Task OnSystemGoAsyncAsync()
    {
        await cs.SystemGoAsync();
    }

    [RelayCommand]
    private async Task OnSystemHaltAsyncAsync()
    {
        await cs.SystemHaltAsync();
    }

    [RelayCommand]
    private async Task OnSystemLocoHaltAsyncAsync()
    {
        await cs.SystemLocoHaltAsync();
    }

    [RelayCommand]
    private async Task OnSystemLocoCycleStopAsyncAsync()
    {
        await cs.SystemLocoCycleStopAsync();
    }

    #endregion

    //[RelayCommand]
    //private async Task OnLocoInfo()
    //{
    //    await cs.ConfigDataLocoInfo();
    //}

    //[RelayCommand]
    //private async Task OnLocos()
    //{
    //    string file = await cs.RequestConfigDataLocos();
    //}

    
}
