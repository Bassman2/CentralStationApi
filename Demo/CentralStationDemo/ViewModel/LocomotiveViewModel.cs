namespace CentralStationDemo.ViewModel;

public partial class LocomotiveViewModel : ObservableObject
{
    //private static MainViewModel mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
    //private readonly CentralStation cs;

    //public LocomotiveViewModel(Locomotive loco, CentralStation cs)
    //{
    //    this.cs = cs;

    //    Name = loco.Name;
    //    Uid = loco.Uid;
    //    MfxUid = loco.MfxUid;
    //    Address = loco.Address;
    //    IconName = loco.IconName;
    //    IconUri = loco.IconUri;
    //    Type = loco.Type;
    //    MfxType = loco.MfxType;
    //    Symbol = loco.Symbol;

    //    Sid = loco.Sid;
    //    MaxSpeed = loco.MaxSpeed;
    //    VMax = loco.VMax;
    //    VMin = loco.VMin;
    //    Av = loco.Av;
    //    Bv = loco.Bv;
    //    Volume = loco.Volume;
    //    Spa = loco.Spa;
    //    Spm = loco.Spm;
    //    Velocity = loco.Velocity;
    //    Direction = loco.Direction;

    //    Functions = [.. loco.Functions!.Select(f => new FunctionViewModel(this, f)).
    //        Concat(     loco.Functions2?.Select(f => new FunctionViewModel(this, f)) ?? [])];
    //    //// http://cs3/app/assets/lok/NS%20186%20012-8.png

    //    //if (!string.IsNullOrEmpty(IconName))
    //    //{
    //    //    Icon = App.Current.Dispatcher.Invoke(() =>
    //    //    {
    //    //        var uri = new Uri($"http://{host}/app/assets/lok/{IconName}.png");
    //    //        return new BitmapImage(uri);
    //    //    });
    //    //}
    //}

    //public LocomotiveViewModel(CentralStation cs, LocomotiveModel loco)
    //{
    //    this.cs = cs;

    //    Name = loco.Name;
    //    Uid = loco.Uid;
    //    MfxUid = loco.MfxUid;
    //    Address = loco.Address;
    //    IconName = loco.IconName;
    //    IconUri = IconName != null ? cs.GetLocoUri(IconName) : null; 
    //    Type = loco.Type;
    //    MfxType = loco.MfxType;
    //    Symbol = loco.Symbol;

    //    Sid = loco.Sid;
    //    MaxSpeed = loco.MaxSpeed;
    //    VMax = loco.VMax;
    //    VMin = loco.VMin;
    //    Av = loco.Av;
    //    Bv = loco.Bv;
    //    Volume = loco.Volume;
    //    Spa = loco.Spa;
    //    Spm = loco.Spm;
    //    Velocity = loco.Velocity;
    //    Direction = loco.Direction;

    //    Functions = loco.Functions!.Select(f => new FunctionViewModel(this, f)).ToList() ?? [];
    //        ;
    //    //// http://cs3/app/assets/lok/NS%20186%20012-8.png

    //    //if (!string.IsNullOrEmpty(IconName))
    //    //{
    //    //    Icon = App.Current.Dispatcher.Invoke(() =>
    //    //    {
    //    //        var uri = new Uri($"http://{host}/app/assets/lok/{IconName}.png");
    //    //        return new BitmapImage(uri);
    //    //    });
    //    //}
    //}

    //public void UpdateLocomotive(CanMessage msg)
    //{
    //    switch (msg.Command)
    //    {
    //    case Command.LocoVelocity:
    //        Velocity = msg.GetDataUShort(4);
    //        break;
    //    case Command.LocoDirection:
    //        DirectionChange directionChange = (DirectionChange)msg.GetDataByte(4);
    //        switch (directionChange)
    //        {
    //        case DirectionChange.Remain:
    //            break;
    //        case DirectionChange.Forward:
    //            break;
    //        case DirectionChange.Backward:
    //            break;
    //        case DirectionChange.Toggle:
    //            break;
    //        case DirectionChange.Unknown4:
    //            break;
    //        case DirectionChange.Unknown5:
    //            break;

    //        default:
    //            throw new NotImplementedException();
    //        }
    //        break;
    //    case Command.LocoFunction:
    //        break;
    //    default:
    //        throw new NotImplementedException();
    //    }
    //}

    public void Halt() => Velocity = 0;

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
        Velocity = 0;
    }
    

    public void SetFunction(byte function, byte value)
    { 
        Debug.WriteLine($"SetFunction Locomotive {Uid} Function {function} Value {value}");
    }

    #region info

    //[ObservableProperty]
    //private ImageSource? icon;

    [ObservableProperty]
    public partial Uri? IconUri { get; set; }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial uint Uid { get; set; }           

    [ObservableProperty]
    public partial uint MfxUid { get; set; }

    [ObservableProperty]
    public partial uint Address { get; set; }   

    [ObservableProperty]
    public partial string? IconName { get; set; }

    [ObservableProperty]
    public partial DecoderType Type { get; set; }

    [ObservableProperty]
    public partial uint MfxType { get; set; }

    [ObservableProperty]
    public partial Symbol Symbol { get; set; }

    [ObservableProperty]
    public partial uint Sid { get; set; }

    [ObservableProperty]
    public partial uint MaxSpeed { get; set; }

    [ObservableProperty]
    public partial uint VMax { get; set; }

    [ObservableProperty]
    public partial uint VMin { get; set; }

    [ObservableProperty]
    public partial uint Av { get; set; }

    [ObservableProperty]
    public partial uint Bv { get; set; }

    [ObservableProperty]
    public partial uint Volume { get; set; }

    [ObservableProperty]
    public partial uint Spa { get; set; }

    [ObservableProperty]
    public partial uint Spm { get; set; }

    [ObservableProperty]
    public partial ushort Velocity { get; set; }

    [ObservableProperty]
    public partial Direction Direction { get; set; }

    [ObservableProperty]
    public partial List<FunctionViewModel> Functions { get; set; }

    #endregion

    #region control

    partial void OnVelocityChanged(ushort value)
    {
        //var direction = Task.Run(async () =>
        //await cs.SetLocomotiveVelocityAsync(Uid, value)).Result;
    }

    [RelayCommand]
    private void OnEmergencyHalt()
    {
        centralStation.SystemLocomotiveEmergencyHaltAsync(Uid).Start();
    }

    [RelayCommand]
    private void OnDirection()
    { 
        //centralStation.SetLocomotiveDirection(Uid, DirectionChange.Toggle);

        var direction = Task.Run(async () => 
        await centralStation.SetLocomotiveDirectionAsync(Uid, DirectionChange.Toggle)).Result;
    }

    [RelayCommand]
    private void OnFunction(string functionNum)
    {

    }

    #endregion

}

