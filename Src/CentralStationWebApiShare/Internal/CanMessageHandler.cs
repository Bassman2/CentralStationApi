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

    public async Task<List<Device>?> SendMessageWithMultipleResponseAsync(CanMessage req, CancellationToken cancellationToken = default)
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

    public async Task<CanMessageCollector?> SendMessageWithCollectorResponseAsync(CanMessage req, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCollectorSource<CanMessageCollector>(new CanMessageCollector());

        DebugInfo($"SendMessageWithCollectorResponseAsync: {req.ToTrace()}");

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
                    var res = await tcs.Task;
                    DebugInfo($"SendMessageWithCollectorResponseAsync return");
                    return res; //  return await tcs.Task;
                }
                catch (OperationCanceledException) 
                {
                    DebugInfo($"SendMessageWithCollectorResponseAsync return: null");
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
            DebugInfo($"OnResponseReceived SoftwareVersion: {msg.ToTrace()}");
            var device = new Device(msg);
            devicesRequests.TryAdd(device.DeviceId, device);
            return; // break next handling
        }

        if (pendingRequests.TryGetValue(msg, out var tcs))
        {
            DebugInfo($"OnResponseReceived: pendingRequests res {msg.ToTrace()}");
            tcs.TrySetResult(msg);
        }

        if (pendingCollectorRequests.TryGetValue(msg, out var collectorTcs))
        {
            var collector = collectorTcs.Result;

            if (msg.Command == Command.StatusData && msg.IsResponse)
            {
                DebugInfo($"OnResponseReceived: HandleStatusData res {msg.ToTrace()}");
                switch (msg.DataLength)
                {
                case 5:
                    break;
                case 6:
                    collectorTcs.TrySetResult(collector);
                    return;
                case 8:
                    //ushort packageIndex = (byte)(msg.Hash & 0xff);
                    collector.AddData(msg.GetData());
                    break;
                default:
                    throw new InvalidDataException($"HandleStatusData DataLength {msg.DataLength} not supported!");
                }
            }
            else if (msg.Command == Command.ConfigData && msg.IsResponse)
            {
                DebugInfo($"OnResponseReceived: HandleConfigData res {msg.ToTrace()}");
                //configDataFileName = msg.GetDataString().Trim('\0');
                return; // break next handling
            }
            // hash compare: the res is for us 
            else if (msg.Command == Command.ConfigDataStream && !msg.IsResponse && msg.Hash == msg.Hash)
            {
                DebugInfo($"OnResponseReceived: HandleConfigDataStream res {msg.ToTrace()}");

                if (msg.DataLength == 6 || msg.DataLength == 7)
                {
                    DebugInfo($"OnResponseReceived: HandleConfigDataStream Length 6 & 7");
                    collector.Length = msg.GetDataUInt(0);
                    collector.Crc = msg.GetDataUShort(4);
                }
                else if (msg.DataLength == 8)
                {
                    DebugInfo($"OnResponseReceived: HandleConfigDataStream Length 8");

                    collector.AddData(msg.GetData());
                    if (collector.IsReady())
                    {
                        DebugInfo($"OnResponseReceived: HandleConfigDataStream Ready");
                        collectorTcs.TrySetResult(collector);
                        return;
                    }
                }
                else
                {
                    DebugInfo($"OnResponseReceived ERROR: Invalid ConfigDataStream res length");
                }
            }
            else DebugInfo($"OnResponseReceived: IGNORE {msg.ToTrace()}");
        }
    }

    [Conditional("DEBUG")]
    private static void DebugInfo(string text) => Debug.WriteLineIf(TraceSwitches.CanMessageHandlerSwitch.TraceInfo, $"{DateTime.Now:HH:mm:ss.ffff} CanMessageHandler: {text}");
   
}
