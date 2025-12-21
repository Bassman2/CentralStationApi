namespace CentralStationWebApi.Internal;

internal class MessageQueue<T> : IDisposable
{
    private readonly Action<T> action;
    private readonly Thread thread;
    private readonly BlockingCollection<T> queue = [];

    public MessageQueue(Action<T> action)
    {
        this.action = action;
        thread = new Thread(WorkLoop) { Name = "MessageQueueThread", IsBackground = true };
        thread.Start();
    }

    public void Dispose()
    {
        queue.Dispose();
    }

    private void WorkLoop()
    {
        foreach (var item in queue.GetConsumingEnumerable())
        {
            try { action(item); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
    }

    public void Add(T item)
    {
        queue.Add(item);
    }
}
