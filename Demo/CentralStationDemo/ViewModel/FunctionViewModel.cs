namespace CentralStationDemo.ViewModel;

public partial class FunctionViewModel : ObservableObject
{
    private LocomotiveViewModel locomotive;

    public FunctionViewModel(LocomotiveViewModel locomotive, Function function)
    {
        this.locomotive = locomotive;
        no = function.Num;
        info = function.Num.ToString();
    }

    public FunctionViewModel(LocomotiveViewModel locomotive, FunctionModel function)
    {
        this.locomotive = locomotive;
        no = function.Num;
        info = function.Num.ToString();
    }

    [ObservableProperty]
    private int no;

    [ObservableProperty]
    private Uri? iconUri;

    [ObservableProperty]
    private string? info;
}
