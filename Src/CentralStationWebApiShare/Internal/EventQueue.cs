namespace CentralStationWebApi.Internal;

internal class EventQueue<T> : IDisposable
{

    private readonly Action<T> action;
    private readonly Thread thread;
    private readonly BlockingCollection<T> queue = [];
    private readonly AutoResetEvent autoResetEvent = new(false);
    private readonly TimeSpan timeout;

    public EventQueue(Action<T> action, TimeSpan timeout)
    {
        this.action = action;
        this.timeout = timeout;
        thread = new Thread(WorkLoop) { Name = "EventQueueThread", IsBackground = true };
        thread.Start();
    }

    public void Dispose()
    {
        autoResetEvent.Dispose();
        queue.Dispose();
    }

    private void WorkLoop()
    {
        foreach (var item in queue.GetConsumingEnumerable())
        {
            try { action(item); autoResetEvent.WaitOne(timeout); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
    }

    public void Add(T item)
    {
        queue.Add(item);
    }

    public void Continue()
    {
        autoResetEvent.Set();
    }
}
