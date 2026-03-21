using System.Collections.Generic;

namespace CentralStationApi.Internal;

internal class CanMessageHandler(CentralStation cs)
{
    private readonly ConcurrentDictionary<CanMessage, TaskCompletionSource<CanMessage>> pendingRequests = new(CanMessageComparer.Instance);

    private readonly ConcurrentDictionary<uint, Device> devicesRequests = [];

    private readonly ConcurrentDictionary<CanMessage, TaskCollectorSource<CanStatusCollector>> pendingStatusCollectorRequests = new(CanStatusComparer.Instance);

    private readonly ConcurrentDictionary<CanMessage, TaskCollectorSource<CanStreamCollector>> pendingStreamCollectorRequests = new(CanStreamComparer.Instance);


    public TimeSpan MessageTimeout { get; set; } = TimeSpan.FromMilliseconds(10000);
    public TimeSpan CollectionTimeout { get; set; } = TimeSpan.FromSeconds(20);

    public async Task<CanMessage?> SendMessageAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<CanMessage>();

        DebugInfo($"SendMessageAsync: {req.ToTrace()}");

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
                    var res = await tcs.Task;
                    DebugInfo($"SendMessageAsync return : {res.ToTrace()}");
                    return res; // await tcs.Task;
                }
                catch (OperationCanceledException) 
                {
                    DebugInfo($"SendMessageAsync return: null");
                    return null;
                }
            }
        }
        finally
        {
            pendingRequests.TryRemove(req, out _);
        }
    }

    public async Task<List<Device>?> SendSoftwareVersionMessageAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        DebugInfo($"SendMessageWithMultipleResponseAsync: {req.ToTrace()}");
        devicesRequests.Clear();
        cs.SendMessage(req);

        try
        {
            await Task.Delay(MessageTimeout, cancellationToken);
            DebugInfo($"SendMessageWithMultipleResponseAsync return");
            return [.. devicesRequests.Values];
        }
        catch (OperationCanceledException)
        {
            DebugInfo($"SendMessageWithMultipleResponseAsync return: null");
            return null;
        }
    }

    public async Task<CanMessageCollector?> SendStatusMessageAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCollectorSource<CanStatusCollector>(new CanStatusCollector(req));

        DebugInfo($"SendMessageWithCollectorResponseAsync: {req.ToTrace()}");

        if (!pendingStatusCollectorRequests.TryAdd(req, tcs))
        {
            throw new InvalidOperationException($"A request is already pending.");
        }

        cs.SendMessage(req);

        // Create a combined cancellation token source for both timeout and cancellation
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        //cts.CancelAfter(CollectionTimeout);
        var ct = cts.Token;

        try
        {
            // Wait for the response with cancellation support
            using (ct.Register(() => tcs.TrySetCanceled(ct)))
            {
                try
                {
                    var res = await tcs.Task;
                    DebugInfo($"SendStatusMessageAsync return");
                    return res; //  return await tcs.Task;
                }
                catch (OperationCanceledException) 
                {
                    DebugInfo($"SendStatusMessageAsync return: null");
                    return null;
                }
            }
        }
        finally
        {
            pendingRequests.TryRemove(req, out _);
        }
    }

    public async Task<CanStreamCollector?> SendStreamMessageAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCollectorSource<CanStreamCollector>(new CanStreamCollector(req));

        DebugInfo($"SendMessageWithCollectorResponseAsync: {req.ToTrace()}");

        if (!pendingStreamCollectorRequests.TryAdd(req, tcs))
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
                    var res = await tcs.Task;
                    DebugInfo($"SendStreamMessageAsync return");
                    return res; //  return await tcs.Task;
                }
                catch (OperationCanceledException)
                {
                    DebugInfo($"SendStreamMessageAsync return: null");
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
        switch (msg.Command)
        {
        case Command.SoftwareVersion when msg.IsResponse:
            DebugInfo($"OnResponseReceived SoftwareVersion: {msg.ToTrace()}");
            devicesRequests.TryAdd(msg.DeviceId, new Device(msg));
            break;
        case Command.StatusData when msg.IsResponse:
            DebugInfo($"OnResponseReceived: {msg.Command} res {msg.ToTrace()}");
            if (pendingStatusCollectorRequests.TryGetValue(msg, out var statusCollectorTcs))
            {
                var collector = statusCollectorTcs.Result;
                if (collector.AddMessage(msg))
                {
                    DebugInfo($"OnResponseReceived: HandleConfigDataStream Ready");
                    statusCollectorTcs.TrySetResult(collector);
                }
            }
            break;
        case Command.ConfigDataStream when !msg.IsResponse:
            DebugInfo($"OnResponseReceived: {msg.Command} res {msg.ToTrace()}");
            if (pendingStreamCollectorRequests.TryGetValue(msg, out var streamCollectorTcs))
            {
                var collector = streamCollectorTcs.Result;
                if (collector.AddMessage(msg))
                {
                    DebugInfo($"OnResponseReceived: HandleConfigDataStream Ready");
                    streamCollectorTcs.TrySetResult(collector);
                }
            }
            break;
        default:
            if (pendingRequests.TryGetValue(msg, out var tcs))
            {
                DebugInfo($"OnResponseReceived: pendingRequests res {msg.ToTrace()}");
                tcs.TrySetResult(msg);
            }
            break;
        }
    }

    [Conditional("DEBUG")]
    private static void DebugInfo(string text) => Debug.WriteLineIf(TraceSwitches.CanMessageHandlerSwitch.TraceInfo, $"{DateTime.Now:HH:mm:ss.ffff} CanMessageHandler: {text}");
   
}
