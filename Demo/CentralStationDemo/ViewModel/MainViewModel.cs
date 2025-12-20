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

}
