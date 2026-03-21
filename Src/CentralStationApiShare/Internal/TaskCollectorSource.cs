namespace CentralStationApi.Internal;

internal class TaskCollectorSource<TResult>(TResult result) : TaskCompletionSource<TResult>
{
    public TResult Result = result;
}
