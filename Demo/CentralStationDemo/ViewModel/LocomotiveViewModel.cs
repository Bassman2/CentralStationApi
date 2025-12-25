using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CentralStationDemo.ViewModel;

public partial class LocomotiveViewModel : ObservableObject
{
    public LocomotiveViewModel(CsLocomotive loco)
    {
        Name = loco.Name;
        Uid = loco.Uid;
        MfxUid = loco.MfxUid;
        Address = loco.Address;
        IconName = loco.Icon;
        Type = loco.Type;
        MfxType = loco.MfxType;

        // http://cs3/app/assets/lok/NS%20186%20012-8.png

        if (!string.IsNullOrEmpty(IconName))
        {
            Icon = DownloadImage();
        }
    }

    private ImageSource DownloadImage()
    {
        return App.Current.Dispatcher.Invoke(() =>
        {
            var uri = new Uri($"http://cs3/app/assets/lok/{IconName}.png");
            return new BitmapImage(uri);
        });
    }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private uint uid;

    [ObservableProperty]
    private uint mfxUid;

    [ObservableProperty]
    private uint address;

    [ObservableProperty]
    private string? iconName;

    [ObservableProperty]
    private ImageSource? icon;

    [ObservableProperty]
    private string? type;

    [ObservableProperty]
    private uint mfxType;
    
}

