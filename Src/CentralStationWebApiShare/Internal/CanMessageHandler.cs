namespace CentralStationWebApi.Internal;

internal class CanMessageHandler(CentralStation cs)
{
    private readonly ConcurrentDictionary<CanMessage, TaskCompletionSource<CanMessage>> pendingRequests =
        new(CanMessageComparer.Instance);

    private readonly ConcurrentDictionary<CanMessage, TaskCollectorSource<CanMessageCollector>> pendingCollectorRequests =
        new(CanMessageCollectorComparer.Instance);

    private readonly ConcurrentDictionary<uint, Device> devicesRequests = [];

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

    public async Task<List<Device>?> SendMessageWithMultipleResponseAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        devicesRequests.Clear();
        cs.SendMessage(req);

        try
        {
            await Task.Delay(MessageTimeout, cancellationToken);
            return [.. devicesRequests.Values];
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public async Task<CanMessageCollector?> SendMessageWithCollectorResponseAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCollectorSource<CanMessageCollector>(new CanMessageCollector());

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
        if (msg.Command == Command.SoftwareVersion && msg.IsResponse)
        {
            var device = new Device(msg);
            devicesRequests.TryAdd(device.DeviceId, device);
            return; // break next handling
        }

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
