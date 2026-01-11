//using System.Windows.Media;

using CentralStationWebApi;
using CentralStationWebApi.Model;

namespace CentralStationDemo.ViewModel;

public partial class LocomotiveViewModel : ObservableObject
{
    //private static MainViewModel mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
    private readonly CentralStation cs;

    public LocomotiveViewModel(Locomotive loco, CentralStation cs)
    {
        this.cs = cs;

        Name = loco.Name;
        Uid = loco.Uid;
        MfxUid = loco.MfxUid;
        Address = loco.Address;
        IconName = loco.IconName;
        IconUri = loco.IconUri;
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


        //// http://cs3/app/assets/lok/NS%20186%20012-8.png

        //if (!string.IsNullOrEmpty(IconName))
        //{
        //    Icon = App.Current.Dispatcher.Invoke(() =>
        //    {
        //        var uri = new Uri($"http://{host}/app/assets/lok/{IconName}.png");
        //        return new BitmapImage(uri);
        //    });
        //}
    }

    public void UpdateLocomotive(CANMessage msg)
    {
        switch (msg.Command)
        {
        case Command.LocoVelocity:
            Velocity = msg.GetDataUShort(4);
            break;
        case Command.LocoDirection:
            DirectionChange directionChange = (DirectionChange)msg.GetDataByte(4);
            switch (directionChange)
            {
            case DirectionChange.Remain:
                break;
            case DirectionChange.Forward:
                break;
            case DirectionChange.Backward:
                break;
            case DirectionChange.Toggle:
                break;
            case DirectionChange.Unknown4:
                break;
            case DirectionChange.Unknown5:
                break;
            default:
                throw new NotImplementedException();
            }
            break;
        case Command.LocoFunction:
            break;
        default:
            throw new NotImplementedException();
        }
    }
    
    public void SetVelocity(ushort velocity) => Velocity = velocity;
    

    public void SetDirection(DirectionChange direction)
    {
        Direction = direction switch
        {
            DirectionChange.Remain => Direction,
            DirectionChange.Forward => Direction.Forward,
            DirectionChange.Backward => Direction.Backward,
            DirectionChange.Toggle => Direction == Direction.Forward ? Direction.Backward : Direction.Forward,
            _ => Direction
        };
    }
    

    public static void SetFunction(byte function, bool value)
    { 
    }

    #region info

    //[ObservableProperty]
    //private ImageSource? icon;

    [ObservableProperty]
    private Uri? iconUri;

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
    private ushort velocity;

    [ObservableProperty]
    private Direction direction;


    #endregion

    #region control

    partial void OnVelocityChanged(ushort value)
    {
        cs.SetLocomotiveVelocity(Uid, value);
    }

    [RelayCommand]
    private void OnEmergencyHalt()
    {
        cs.SystemLocomotiveEmergencyHalt(Uid);
    }

    [RelayCommand]
    private void OnDirection()
    { 
        cs.SetLocomotiveDirection(Uid, DirectionChange.Toggle);
    }

    #endregion

}

