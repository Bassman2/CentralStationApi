using CentralStationWebApi.Serializer;
using System.IO;
using System.Reflection;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;

namespace CentralStationDemo.ViewModel;


// http://cs3/images/gui/default_lok.svg
// http://cs3/images/gui/menue_platten.svg
// http://cs3/images/gui/menue_ereignis.svg
// http://cs3/images/gui/artikel_wahl_weichen.svg
// http://cs3/images/gui/dashboard_desktop.png
// http://cs3/images/gui/dashboard_smartphone.png
// http://cs3/images/gui/btn_assistent2.svg
// http://cs3/images/gui/dashboard_info.svg

// http://cs3/images/gui/dashboard_cs1.png
// http://cs3/images/gui/dashboard_cs2.png
// http://cs3/images/gui/dashboard_cs3.png
// http://cs3/images/gui/dashboard_gfp3.png
// http://cs3/images/gui/dashboard_ms1.png
// http://cs3/images/gui/dashboard_ms2.png
// http://cs3/images/gui/dashboard_links88.png

public sealed partial class MainViewModel : AppViewModel, IDisposable
{
    private const string host = "CS3";
   
    private readonly static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyTitleAttribute>()!.Title);
    private readonly static string appFilesPath = Path.Combine(appDataPath, "Files");
    private readonly static string appCachePath = Path.Combine(appDataPath, "Cache");

    private readonly static string locomotivesFileName = "lokomotive.cs2";
    private readonly static string articlesFileName = "magnetartikel.cs2";
    private readonly static string routesFileName = "fahrstrassen.cs2";
    private readonly static string tracksFileName = "gleisbild.cs2";

    private readonly static string locomotivesStateFileName = "lokomotive.sr2";
    private readonly static string articlesStateFileName = "magnetartikel.sr2";
    private readonly static string routesStateFileName = "fahrstrassen.sr2";
    private readonly static string tracksStateFileName = "gleisbild.sr2";

    private readonly static string locomotivesDBFileName = "lokomotive.db";
    private readonly static string locomotivesDBVersionFileName = "lokomotive-db-version.txt";
    private readonly static string languageFileName = "language.bin";



    private readonly CentralStation cs;

    public string Host => host;

    public MainViewModel()
    {
        cs = new CentralStation(host, Protocol.TCP);
        cs.MessageReceived += (s, e) => App.Current.Dispatcher.Invoke(() => Messages.Insert(0, e.Message));
        cs.PropertyChanged += (s, e) => OnCsPropertyChanged(e.PropertyName);

        cs.LocomotiveHalt += (s, e) => OnLocomotiveHalt(e.LocomotiveId);
        cs.LocomotiveVelocity += (s,e) => OnLocomotiveVelocity(e.LocomotiveId, e.Velocity);
        cs.LocomotiveDirection += (s, e) => OnLocomotiveDirection(e.LocomotiveId, e.Direction); 
        cs.LocomotiveFunction += (s, e) => OnLocomotiveFunction(e.LocomotiveId, e.Function, e.Value);

        cs.FileReceived += (s, e) => OnFileReceived(e.FileName, e.Stream);

    }

    public CentralStation CentralStation => cs;

    public void Dispose() => cs.Dispose();

    protected override void OnStartup()
    {
        Directory.CreateDirectory(appFilesPath);
        Directory.CreateDirectory(appCachePath);

        // load locomotive data 
        Locomotives = LoadFile<LocomotiveData>(locomotivesFileName)?.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        
        // load articles data 
        Articles = LoadFile<ArticleData>(articlesFileName)?.Articles?.ToViewModelList<ArticleViewModel>(cs);
        
        // load routes data 
        Routes = LoadFile<RouteData>(routesFileName)?.Routes?.ToViewModelList<RouteViewModel>();
        
        // load tracks data 
        TrackData = LoadFile<TrackData>(tracksFileName);
        TrackPages = [];
        foreach (var page in TrackData?.Pages ?? [])
        {
            string pageFilePath = Path.Combine(appFilesPath, $"gleisbild-{page.Id}.cs2");

            var trackPage = LoadFile<TrackPageData>(pageFilePath);
            TrackPages.Add(new TrackPageViewModel(page, trackPage));
        }
    }

    private static T? LoadFile<T>(string fileName) where T : ICsSerialize, new()
    {
        string filePath = Path.Combine(appFilesPath, fileName);
        if (File.Exists(filePath))
        {
            using var file = File.OpenRead(filePath);
            return CsSerializer.Deserialize<T>(file);
        }
        return default;
    }

    private static void StoreFile(string fileName, Stream stream)
    {
        string filePath = Path.Combine(appFilesPath, fileName);
        using var file = File.Create(filePath);
        stream.CopyTo(file);
    }


    private void OnCsPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
        case "SystemStatus":
            Status = cs.Status;
            break;
        }
    }

    private void OnFileReceived(string fileName, System.IO.Stream stream)
    {
        stream.Position = 0;
        using (var file = File.Create(fileName))
        {
            stream.CopyTo(file);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [ObservableProperty]
    private double zoomValue = 1.0;


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Status

    [ObservableProperty]
    private SystemStatus status = SystemStatus.Stop;


    [RelayCommand]
    private async Task OnStop()
    {
        CancellationTokenSource cts = new();

        bool res;
            switch (Status)
            {
            case SystemStatus.Default: 
                res = await cs.SystemStopAsync(CentralStation.AllDevices, cts.Token); 
                break;
            case SystemStatus.Go: 
                res = await cs.SystemStopAsync(CentralStation.AllDevices, cts.Token); 
                break;
            case SystemStatus.Stop: 
                res = await cs.SystemGoAsync(CentralStation.AllDevices, cts.Token); 
                break;
            default: 
                throw new InvalidEnumArgumentException(nameof(Status), (int)Status, typeof(SystemStatus));
            }
            Debug.WriteLine($"SystemStop/Go: {res}");
       
    }


    #endregion

    #region System Status


    [RelayCommand]
    private async Task OnUpdateSystemStatus()
    {
        var devices = await cs.GetDevicesAsync();
        var gfpDevice = devices?.FirstOrDefault(d => d.DeviceType == DeviceType.GFP || d.DeviceType == DeviceType.GFP3);
        if (gfpDevice == null)
        {
            MessageBox.Show("No GFP found");
            return;
        }

        for (byte i = 0; i < 100; i++)
        {
            var res = await cs.GetSystemStatusAsync(gfpDevice.DeviceId, i);
            Debug.WriteLine($"SystemStatus {i}: {res}");
        }
    }

    
    public TrackFormatProcessorViewModel TrackFormatProcessor => new(this); 

    #endregion

    #region Locomotives

    [ObservableProperty]
    private List<LocomotiveViewModel>? locomotives;

    [RelayCommand]
    private async Task OnUpdateLocomotives()
    {
        using var stream = await cs.GetConfigDataAsync("loks");
        if (stream is not null)
        {
            StoreFile(locomotivesFileName, stream);
            var data = CsSerializer.Deserialize<LocomotiveData>(stream);
            Locomotives = data.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        }
    }

    [RelayCommand]
    private async Task OnUpdateLocomotivesState()
    {
        using var stream = await cs.GetConfigDataAsync("lokstat");
        if (stream is not null)
        {
            StoreFile(locomotivesStateFileName, stream);
            //var data = CsSerializer.Deserialize<LocomotiveData>(stream);
            //Locomotives = data.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        }
    }

    [RelayCommand]
    private async Task OnUpdateLocomotivesDB()
    {
        using var stream = await cs.GetConfigDataAsync("lokdb");
        if (stream is not null)
        {
            StoreFile(locomotivesDBFileName, stream);
            //var data = CsSerializer.Deserialize<LocomotiveData>(stream);
            //Locomotives = data.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        }
    }

    [RelayCommand]
    private async Task OnUpdateLocomotivesDBVersion()
    {
        using var stream = await cs.GetConfigDataAsync("ldbver");
        if (stream is not null)
        {
            StoreFile(locomotivesDBVersionFileName, stream);
            //var data = CsSerializer.Deserialize<LocomotiveData>(stream);
            //Locomotives = data.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        }
    }

    [RelayCommand]
    private async Task OnUpdateLanguage()
    {
        using var stream = await cs.GetConfigDataAsync("lang");
        if (stream is not null)
        {
            StoreFile(languageFileName, stream);
            //var data = CsSerializer.Deserialize<LocomotiveData>(stream);
            //Locomotives = data.Locomotives?.ToViewModelList<LocomotiveViewModel>(cs);
        }
    }

    [RelayCommand]
    private void OnSortLocomotives(string propertyName)
    {
        CollectionViewSource.GetDefaultView(Locomotives).SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
    }

    private void OnLocomotiveHalt(uint locomotiveId)
    {
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.Halt();
    }
    private void OnLocomotiveVelocity(uint locomotiveId, ushort velocity)
    {   
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.SetVelocity(velocity);
    }

    private void OnLocomotiveDirection(uint locomotiveId, DirectionChange direction)
    { 
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.SetDirection(direction);
    }

    private void OnLocomotiveFunction(uint locomotiveId, byte function, byte value)
    {
        Locomotives?.FirstOrDefault(l => l.Uid == locomotiveId)?.SetFunction(function, value);
    } 

    #endregion

    #region Articles

    [ObservableProperty]
    private List<ArticleViewModel>? articles;

    [RelayCommand]
    private async Task OnUpdateArticles()
    {
        using var stream = await cs.GetConfigDataAsync("mags");
        if (stream is not null)
        {
            StoreFile(articlesFileName, stream);
            var data = CsSerializer.Deserialize<ArticleData>(stream);
            Articles = data.Articles?.ToViewModelList<ArticleViewModel>(cs);
        }
    }

    [RelayCommand]
    private async Task OnUpdateArticlesState()
    {
        using var stream = await cs.GetConfigDataAsync("magstat");
        if (stream is not null)
        {
            StoreFile(articlesStateFileName, stream);
            //var data = CsSerializer.Deserialize<ArticleData>(stream);
            //Articles = data.Articles?.ToViewModelList<ArticleViewModel>(cs);
        }
    }

    #endregion

    #region Routes

    [ObservableProperty]
    private List<RouteViewModel>? routes;

    [RelayCommand]
    private async Task OnUpdateRoutes()
    {
        using var stream = await cs.GetConfigDataAsync("fs");
        if (stream is not null)
        {
            StoreFile(routesFileName, stream);
            var data = CsSerializer.Deserialize<RouteData>(stream);
            Routes = data.Routes?.ToViewModelList<RouteViewModel>();
        }
    }

    [RelayCommand]
    private async Task OnUpdateRoutesState()
    {
        using var stream = await cs.GetConfigDataAsync("fsstat");
        if (stream is not null)
        {
            StoreFile(routesStateFileName, stream);
            //var data = CsSerializer.Deserialize<RouteData>(stream);
            //Routes = data.Routes?.ToViewModelList<RouteViewModel>();
        }
    }

    #endregion

    #region Tracks

    [ObservableProperty]
    private TrackData? trackData;

    [ObservableProperty]
    private ObservableCollection<TrackPageViewModel>? trackPages;

    [RelayCommand]
    private async Task OnUpdateTracks()
    {
        using var stream = await cs.GetConfigDataAsync("gbs");
        if (stream is not null)
        {
            StoreFile(tracksFileName, stream);
            TrackData = CsSerializer.Deserialize<TrackData>(stream);
            TrackPages = [];
            foreach (var page in TrackData.Pages ?? [])
            {

                using var pageStream = await cs.GetConfigDataAsync($"gbs-{page.Id}");
                if (pageStream is not null)
                {
                    StoreFile($"gleisbild-{page.Id}.cs2", pageStream);
                    var data = CsSerializer.Deserialize<TrackPageData>(stream);
                    App.Current.Dispatcher.Invoke(() => TrackPages.Add(new TrackPageViewModel(page, data)));
                }
            }
        }
    }

    [RelayCommand]
    private async Task OnUpdateTracksState()
    {

        using var stream = await cs.GetConfigDataAsync("gbsstat");
        if (stream is not null)
        {
            StoreFile(tracksStateFileName, stream);
            //TrackData = CsSerializer.Deserialize<TrackData>(stream);
            //TrackPages = [];
            //foreach (var page in TrackData.Pages ?? [])
            //{

            //    using var pageStream = await cs.GetConfigDataAsync($"gbs-{page.Id}");
            //    if (pageStream is not null)
            //    {
            //        StoreFile($"gleisbild-{page.Id}.cs2", pageStream);
            //        var data = CsSerializer.Deserialize<TrackPageData>(stream);
            //        App.Current.Dispatcher.Invoke(() => TrackPages.Add(new TrackPageViewModel(page, data)));
            //    }
            //}
        }
    }

    #endregion

    #region Devices

    [ObservableProperty]
    private List<DeviceViewModel>? devices;

    [RelayCommand]
    private async Task OnUpdateDevices()
    {
        var devices = await cs.GetDevicesAsync();

        var vms = devices?.Select(d => new DeviceViewModel(d)).ToList();
        App.Current.Dispatcher.Invoke(() => Devices = vms);

        foreach (var device in Devices ?? [])
        {
            var deviceInfo = await cs.GetDeviceInfoAsync(device.DeviceId);
            if (deviceInfo != null)
            {
                device.AddDeviceInfo(deviceInfo);
            }
        }

        foreach (var device in Devices ?? [])
        {
            for (byte index = 1; index < 6; index++)
            {
                var deviceMeasurement = await cs.GetDeviceMeasurementAsync(device.DeviceId, index);
                //var deviceMeasurement = new DeviceMeasurement() { Name = $"Measurement {index}" };
                if (deviceMeasurement != null)
                {
                    App.Current.Dispatcher.Invoke(() => device.AddDeviceMeasurement(deviceMeasurement, index));
                }
            }
        }
    }

    [RelayCommand]
    private void OnSortControllers(string propertyName)
    {
        CollectionViewSource.GetDefaultView(Devices).SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
    }

    #endregion

    #region Messages

    [ObservableProperty]
    private ObservableCollection<CanMessage> messages = [];

    #endregion
}
