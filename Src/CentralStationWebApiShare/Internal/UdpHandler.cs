using System.Net.Sockets;

namespace CentralStationWebApi.Internal;

internal class UdpHandler : IProtocolHandler, IDisposable
{
    private const int UDPPortSend = 15731;
    private const int UDPPortReceive = 15730;

    private readonly UdpClient sender;
    private readonly UdpClient listener;

    public UdpHandler()
    {
        listener = new UdpClient(UDPPortReceive);
        sender = new UdpClient();
    }
    public void Connect(string host)
    {
        sender.Connect(host, UDPPortSend);
    }

    public void Dispose()
    {
        sender.Close();
        sender.Dispose();

        listener.Close();
        listener.Dispose();
    }

    public void Send(CANMessage msg)
    {
        sender.Send(msg.Buffer, 13); 
    }

    public async Task SendAsync(CANMessage msg)
    {
        await sender.SendAsync(msg.Buffer, 13);
    }

    public async Task<CANMessage> ReceiveAsync()
    {
        var result = await listener.ReceiveAsync();
        return new CANMessage(result);
    }


}
