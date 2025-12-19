using CentralStationWebApi.Internal;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace CentralStationWebApi;

public sealed class CentralStation : IDisposable
{

    private UdpClient sender;
    private UdpClient listener;
    private Task receiver;
    private string host;

    public CentralStation(string host = "CS3") 
    {
        this.host = host;
        this.sender = new UdpClient(15731);
        this.listener = new UdpClient(15730);
        this.receiver = Task.Run(async () => await ReceiveAsync());
    }

    public void Dispose()
    {
        sender.Close();
        sender.Dispose();
    }

    private async Task ReceiveAsync()
    {         
        try
        {
            while (true)
            {
                var result = await listener.ReceiveAsync();
                var text = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine($"Received from {result.RemoteEndPoint}: {text}");
            }
        }
        catch (ObjectDisposedException)
        {
            // expected on disposal
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Receiver error: {ex}");
        }
    }

    public async Task<bool> SystemStopAsync(CancellationToken cancellationToken = default)
    {   
        var message = new SystemMessage(SystemCommand.SystemStop, 0);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemGoAsync(CancellationToken cancellationToken = default)                            
    {
        await Task.Delay(1000, cancellationToken);  
        return true;
    }

    public async Task<bool> SystemHaltAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);
        return true;
    }

    private async Task SendMessageAsync(CANMessage message, CancellationToken cancellationToken = default)
    {
        Debug.WriteLine(message);
        int x = await sender.SendAsync(message.Data, 13, host, 15731);
    }

}



//var message = Encoding.UTF8.GetBytes("Hello from CentralStation");

//// send to localhost:5000
//await udp.SendAsync(message, message.Length, "127.0.0.1", 5000);

//            using var public void Dispose()
//    {
//        throw new NotImplementedException();
//    }
//}

//cts = new CentralStation();