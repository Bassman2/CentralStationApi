using CentralStationWebApi.Model;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CentralStationDemo.ViewModel;

public partial class LocomotiveViewModel : ObservableObject
{
    public LocomotiveViewModel(string host, Locomotive loco)
    {
        Name = loco.Name;
        Uid = loco.Uid;
        MfxUid = loco.MfxUid;
        Address = loco.Address;
        IconName = loco.Icon;
        Type = loco.Type;
        MfxType = loco.MfxType;
        Symbol = loco.Symbol;

        Sid = loco.Sid;
        MaxSpeed = loco.MaxSpeed;
        VMax = loco.VMax;
        VMin = loco.VMin;
        Av = loco.Av;
        Bv = loco.Bv;
        Volume = loco.Volume;
        Spa = loco.Spa;
        Spm = loco.Spm;
        Velocity = loco.Velocity;
        Direction = loco.Direction;


        // http://cs3/app/assets/lok/NS%20186%20012-8.png

        if (!string.IsNullOrEmpty(IconName))
        {
            Icon = App.Current.Dispatcher.Invoke(() =>
            {
                var uri = new Uri($"http://{host}/app/assets/lok/{IconName}.png");
                return new BitmapImage(uri);
            });
        }
    }

    #region info

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
    private DecoderType type;

    [ObservableProperty]
    private uint mfxType;

    [ObservableProperty]
    private Symbol symbol;

    [ObservableProperty]
    private uint sid;

    [ObservableProperty]
    private uint maxSpeed;

    [ObservableProperty]
    private uint vMax;

    [ObservableProperty]
    private uint vMin;

    [ObservableProperty]
    private uint av;

    [ObservableProperty]
    private uint bv;

    [ObservableProperty]
    private uint volume;

    [ObservableProperty]
    private uint spa;

    [ObservableProperty]
    private uint spm;

    [ObservableProperty]
    private uint velocity;


    [ObservableProperty]
    private uint direction;


    #endregion

    #region control

    [ObservableProperty]
    private uint speed;

    [RelayCommand]
    private void OnEmergencyHalt()
    {

    }

    #endregion

}

