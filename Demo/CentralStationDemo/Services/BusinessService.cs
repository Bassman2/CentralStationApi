
using CentralStationDemo.Model;

namespace CentralStationDemo.Services;

public class BusinessService : IBusinessService
{
    private readonly CentralStation centralStation;

    private readonly CentralStationModel centralStationModel;

    public BusinessService()
    {
        centralStationModel = CentralStationModel.LoadOrCreate();
        centralStation = new CentralStation("CS3", Protocol.TCP);
    }

    public void Dispose()
    {
        centralStation.Dispose();
    }

    public List<LocomotiveViewModel>? Locomotives { get; set; }

    public List<ArticleViewModel>? Articles { get; set; }

    public List<RouteViewModel>? Routes { get; set; }

    public List<TrackPageViewModel>? TrackPages { get; set; }

    public List<DeviceViewModel>? Devices { get; set; }
        

    public async Task UpdateArticlesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateDevicesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateLocomotivesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateRoutesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateTrackPagesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
}
