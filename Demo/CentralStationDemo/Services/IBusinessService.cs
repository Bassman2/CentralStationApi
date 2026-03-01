namespace CentralStationDemo.Services;

public interface IBusinessService : IDisposable
{
    List<LocomotiveViewModel>? Locomotives { get; }

    List<ArticleViewModel>? Articles { get; }

    List<RouteViewModel>? Routes { get; }
        
    List<TrackPageViewModel>? TrackPages { get; }

    List<DeviceViewModel>? Devices { get; }

    Task UpdateLocomotivesAsync(CancellationToken cancellationToken = default);

    Task UpdateArticlesAsync(CancellationToken cancellationToken = default);

    Task UpdateRoutesAsync(CancellationToken cancellationToken = default);

    Task UpdateTrackPagesAsync(CancellationToken cancellationToken = default);

    Task UpdateDevicesAsync(CancellationToken cancellationToken = default);
}
