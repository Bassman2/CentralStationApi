namespace CentralStationDemo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void OnApplicationStartup(object sender, StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, a) =>
        {
            Exception ex = (Exception)a.ExceptionObject;
            Trace.TraceError(ex.ToString());
            MessageBox.Show(ex.ToString(), "Unhandled Error !!!");
        };

        Ioc.Default.ConfigureServices
        (
            new ServiceCollection()
                //.AddSingleton<IBusinessLogic, BusinessLogic>()
                //.AddSingleton<DialogService, DialogService>()
                .AddScoped<MainViewModel>()
                //.AddScoped<UsersViewModel>()
                //.AddScoped<GroupsViewModel>()
                //.AddScoped<MembersViewModel>()
                //.AddScoped<EditViewModel>()
                //.AddScoped<SelectUsersViewModel>()
                //.AddScoped<SelectGroupsViewModel>()
                .BuildServiceProvider()
        );

        new CentralStationDemo.View.MainView().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        //Ioc.Default.GetRequiredService<IBusinessLogic>().Dispose();
        base.OnExit(e);
    }
}
