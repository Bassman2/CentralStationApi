using System.IO;

namespace CentralStationDemo.ViewModel;

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private const string host = "CS3";
    private CentralStation cs; 
    public MainViewModel()
    {
        cs = new CentralStation(host);
        cs.MessageReceived += (s, e) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Messages.Insert(0, new MessageViewModel(e.Message));

                UpdateStatus(e.Message);
            });
        };
        cs.PropertyChanged += (s, e) => OnCsPropertyChanged(e.PropertyName);
        
        //cs.FileReceived += (s, e) =>
        //{
        //    ScanStreams(e.CSFileStream.GetFileStream()); 
        //};
    }

    public void Dispose()
    {
        //cs.Close();
        cs.Dispose();
    }

    protected override void OnStartup()
    {
        cs.RequestConfigDataLocos();
        cs.RequestConfigDataMagneticItems();
        cs.RequestConfigDataRailwayRoute();
        cs.RequestConfigDataTrackDiagramRoute();
    }

    private void UpdateStatus(CANMessage message)
    { 
        if (message.Command == Command.SystemCommand && message.SystemCommand == SystemCommand.Stop && message.Device == CentralStation.AllDevices)
        {
            SystemStatus = SystemStatus.Stop;
        }
        if (message.Command == Command.SystemCommand && message.SystemCommand == SystemCommand.Go && message.Device == CentralStation.AllDevices)
        {
            SystemStatus = SystemStatus.Go;
        }
    }

    private void OnCsPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
        case "Locomotives":
            Locomotives = cs.Locomotives?.Locomotives_?.Select(i => new LocomotiveViewModel(host, i))?.ToList(); 
            break;
        case "MagneticItems":
            MagneticItems = cs.MagneticItems?.Articles?.Select(i => new MagneticItemViewModel(i))?.ToList();
            break;
        case "RailwayRoutes":
            RailwayRoutes = cs.RailwayRoutes?.RailwayRoutes_?.Select(i => new RailwayRouteViewModel(i))?.ToList();
            break;
        case "TrackDiagram":
            TrackDiagram = cs.TrackDiagram;
            break;

        }
    }

    private void ScanStreams(Stream stream)
    {
        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string? line = reader.ReadLine();

        switch (line)
        {
        case "[lokomotive]":
            Locomotives = CsSerializer.Deserialize<Locomotives>(stream, "[lokomotive]").Locomotives_?.Select(i => new LocomotiveViewModel(host, i))?.ToList(); 
            break;
        case "[magnetartikel]":
            MagneticItems = CsSerializer.Deserialize<MagneticItems>(stream, "[magnetartikel]")?.Articles?.Select(i => new MagneticItemViewModel(i))?.ToList();
            break;
        case "[fahrstrassen]":
            RailwayRoutes = CsSerializer.Deserialize<RailwayRoutes>(stream, "[fahrstrassen]")?.RailwayRoutes_?.Select(i => new RailwayRouteViewModel(i))?.ToList();
            break;
        case "[gleisbild]":
            TrackDiagram = CsSerializer.Deserialize<TrackDiagram>(stream, "[gleisbild]]");
            break;
        }
            
    }

    [ObservableProperty]
    private SystemStatus systemStatus = SystemStatus.Default;

    [ObservableProperty]
    private uint device = 0;

    //partial void OnDeviceChanged(uint value)
    //{ 
    //}

    [ObservableProperty]
    private List<LocomotiveViewModel>? locomotives;

    [ObservableProperty]
    private List<MagneticItemViewModel>? magneticItems;

    [ObservableProperty]
    private List<RailwayRouteViewModel>? railwayRoutes;

    [ObservableProperty]
    private ObservableCollection<MessageViewModel> messages = [];

    [ObservableProperty]
    private TrackDiagram? trackDiagram;

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
