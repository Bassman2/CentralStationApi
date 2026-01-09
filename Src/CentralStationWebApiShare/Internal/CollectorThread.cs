namespace CentralStationWebApi.Internal;

internal sealed class CollectorThread : IDisposable
{
    private readonly TimeSpan timeout;
    private readonly Action action;
    //private readonly Task task;
    private readonly Thread thread;
    private readonly AutoResetEvent autoResetEvent = new(false);

    private bool isRunning = true;

    public CollectorThread(Action action, TimeSpan timeout)
    {
        this.action = action;
        this.timeout = timeout;
        
        //this.task = Task.Run(WorkLoop);

        thread = new Thread(WorkLoop) { Name = "EventQueueThread", IsBackground = true };
        thread.Start();
    }
    
    public void Dispose()
    {
        isRunning = false;
        autoResetEvent.Set();
        //task.Wait();
    }

    private void WorkLoop()
    {
        while (autoResetEvent.WaitOne(timeout))
        {
            if (!isRunning) return;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    public void Next()
    {
        autoResetEvent.Set();
    }
}
