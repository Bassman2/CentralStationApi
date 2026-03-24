using System.Net;

namespace CentralStationApi.Internal;

internal sealed class UdpHandler : IProtocolHandler, IDisposable
{
    private const int UDPPortSend = 15731;
    private const int UDPPortReceive = 15730;

    private readonly UdpClient senderClient;
    private readonly UdpClient listenerClient;

    private string sender = "unknown";

    

    public UdpHandler()
    {
        listenerClient = new UdpClient(UDPPortReceive);
        senderClient = new UdpClient();

    }
    public void Connect(string host)
    {
        senderClient.Connect(host, UDPPortSend);

        sender = Dns.GetHostEntry(host).HostName.Split('.')[0];
    }

    public bool IsConnected => senderClient.Client.Connected && listenerClient.Client.Connected;

    public void Dispose()
    {
        senderClient.Close();
        senderClient.Dispose();

        listenerClient.Close();
        listenerClient.Dispose();
    }

    public void Send(CanMessage msg)
    {
        senderClient.Send(msg.Buffer, 13); 
    }

    public async Task SendAsync(CanMessage msg)
    {
        await senderClient.SendAsync(msg.Buffer, 13);
    }

    public async Task<CanMessage> ReceiveAsync()
    {
        var result = await listenerClient.ReceiveAsync();
        if (result.Buffer.Length != 13)
        {
            throw new InvalidDataException($"Invalid CAN message length: {result.Buffer.Length}");
        }
        return new CanMessage(sender, result.Buffer);
    }


}
