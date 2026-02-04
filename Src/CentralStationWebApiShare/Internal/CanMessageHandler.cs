namespace CentralStationWebApi.Internal;

internal class CanMessageHandler(CentralStation cs)
{
    private readonly ConcurrentDictionary<CanMessage, TaskCompletionSource<CanMessage>> pendingRequests =
        new(CanMessageComparer.Instance);

    public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 0, 500);

    public async Task<CanMessage?> SendMessageAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<CanMessage>();

        if (!pendingRequests.TryAdd(req, tcs))
        {
            throw new InvalidOperationException($"A request is already pending.");
        }

        //try
        //{
        cs.SendMessage(req);

        // Create a combined cancellation token source for both timeout and cancellation
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(Timeout);
        var ct = cts.Token;
        
        try
        { 
            // Wait for the response with cancellation support
            using (ct.Register(() => tcs.TrySetCanceled(ct)))
            {
                try
                {
                    return await tcs.Task;
                }
                catch (OperationCanceledException) when (cts.IsCancellationRequested == true && !cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
            }
        }
        finally
        {
            pendingRequests.TryRemove(req, out _);
        }
    }

    //private async Task SendMessageInternalAsync(CanMessage msg, CancellationToken cancellationToken)
    //{
    //    // TODO: Replace with your actual message sending implementation
    //    // Examples:
    //    // - Send via HTTP client
    //    // - Send via message queue
    //    // - Send via WebSocket
    //    // - Send via SignalR
    //    cs.SendMessage(CentralStation
    //    await Task.CompletedTask;
    //}

    public void OnResponseReceived(CanMessage msg)
    {
        if (pendingRequests.TryGetValue(msg, out var tcs))
        {
            tcs.TrySetResult(msg);
        }
    }

}
