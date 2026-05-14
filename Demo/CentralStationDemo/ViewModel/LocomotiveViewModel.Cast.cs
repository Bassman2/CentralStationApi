namespace CentralStationDemo.ViewModel;

partial class LocomotiveViewModel
{
    private readonly CentralStation centralStation;

    public LocomotiveViewModel(LocomotiveModel model, CentralStation centralStation)
    {
        this.centralStation = centralStation;

        Name = model.Name;
        Uid = model.Uid;
        MfxUid = model.MfxUid;
        Address = model.Address;
        IconName = model.IconName;
        //IconUri = model.IconUri;
        Type = model.Type;
        MfxType = model.MfxType;
        Symbol = model.Symbol;

        Sid = model.Sid;
        MaxSpeed = model.MaxSpeed;
        VMax = model.VMax;
        VMin = model.VMin;
        Av = model.Av;
        Bv = model.Bv;
        Volume = model.Volume;
        Spa = model.Spa;
        Spm = model.Spm;
        Velocity = model.Velocity;
        Direction = model.Direction;

        //Functions = [.. loco.Functions!.Select(f => new FunctionViewModel(this, f)).
        //        Concat(     loco.Functions2?.Select(f => new FunctionViewModel(this, f)) ?? [])];
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

    public static List<LocomotiveViewModel> FromModels(IEnumerable<LocomotiveModel>? models, CentralStation centralStation)
        => models?.Select(model => new LocomotiveViewModel(model, centralStation)).ToList() ?? [];
}

static class LocomotiveViewModelStaticConverter
{
    public static List<LocomotiveViewModel> FromModels(this IEnumerable<LocomotiveModel>? models, CentralStation centralStation)
        => models?.Select(model => new LocomotiveViewModel(model, centralStation)).ToList() ?? [];
}