using CentralStationWebApi.Internal;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace CentralStationWebApi;

public sealed class CentralStation : IDisposable
{
    private const int PortSend = 15731;
    private const int PortReceive = 15730;

    private UdpClient sender;
    private UdpClient listener;
    private Task receiver;
    private string host;

    public CentralStation(string host = "CS3") 
    {
        this.host = host;

        this.listener = new UdpClient(PortReceive);
        
        //this.listener.Connect(host, PortReceive);

        this.receiver = Task.Run(async () => await ReceiveAsync());

        this.sender = new UdpClient();
        this.sender.Connect(host, PortSend);

    }

    public void Dispose()
    {
        sender.Close();
        sender.Dispose();
    }

    private async Task SendMessageAsync(CANMessage message, CancellationToken cancellationToken = default)
    {
        Debug.WriteLine($"Send: {message}");
        //int x = await sender.SendAsync(message.Data, 13, host, 15731);
        int x = await sender.SendAsync(message.Data, 13);
    }
    private async Task ReceiveAsync()
    {         
        try
        {
            while (true)
            {
                var result = await listener.ReceiveAsync();
                var msg = new CANMessage(result.Buffer);
                Debug.WriteLine($"Received: {msg}");
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