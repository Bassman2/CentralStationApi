namespace CentralStationWebApi.Internal;

internal class CanMessageHandler(CentralStation cs)
{
    private readonly ConcurrentDictionary<CanMessage, TaskCompletionSource<CanMessage>> pendingRequests =
        new(CanMessageComparer.Instance);

    private readonly ConcurrentDictionary<CanMessage, TaskCompletionSource<CanMessageCollector>> pendingCollectorRequests =
        new(CanMessageComparer.Instance);
    

    public TimeSpan MessageTimeout { get; set; } = TimeSpan.FromMilliseconds(500);
    public TimeSpan CollectionTimeout { get; set; } = TimeSpan.FromSeconds(2);

    public async Task<CanMessage?> SendMessageAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<CanMessage>();

        if (!pendingRequests.TryAdd(req, tcs))
        {
            throw new InvalidOperationException($"A request is already pending.");
        }
                
        cs.SendMessage(req);

        // Create a combined cancellation token source for both timeout and cancellation
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(MessageTimeout);
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
                catch (OperationCanceledException) 
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

    public async Task<CanMessageCollector?> SendMessageWithCollectorResponseAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<CanMessageCollector>();

        if (!pendingCollectorRequests.TryAdd(req, tcs))
        {
            throw new InvalidOperationException($"A request is already pending.");
        }

        cs.SendMessage(req);

        // Create a combined cancellation token source for both timeout and cancellation
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(CollectionTimeout);
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
                catch (OperationCanceledException) 
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

    public void OnResponseReceived(CanMessage msg)
    {
        if (pendingRequests.TryGetValue(msg, out var tcs))
        {
            tcs.TrySetResult(msg);
        }

        if (pendingCollectorRequests.TryGetValue(msg, out var collectorTcs))
        {
            var collector = new CanMessageCollector();
            collector.AddData(msg.GetData());

            collectorTcs.TrySetResult(collector);
        }
    }

}
