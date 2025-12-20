using WpfToolbox.View;

namespace CentralStationDemo.View;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : AppWindowView
{
    public MainView()
    {
        InitializeComponent();
        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
        }
    }
}
