using System.Net;

namespace CentralStationWebApi.Internal;

internal class TcpHandler : IProtocolHandler, IDisposable
{
    private const int CANMsgLength = 13;
    private const int TCPPort = 15731;

    private readonly TcpClient client;
    private NetworkStream? stream;

    private string sender = "unknown";

    public TcpHandler()
    {
        client = new TcpClient();
        client.ReceiveTimeout = 5 * 60 * 1000;
    }

    public void Connect(string host)
    {
        client.Connect(host, TCPPort);
        stream = client.GetStream();

        sender = Dns.GetHostEntry(host).HostName.Split('.')[0];
    }

    public void Dispose()
    {
        client.Close();
        client.Dispose();

        if (stream is not null)
        {
            stream.Close();
            stream.Dispose();
        }
    }
    
    public void Send(CANMessage msg)
    {
        stream!.Write(msg.Buffer, 0, CANMsgLength);
    }

    public async Task SendAsync(CANMessage msg)
    {
        await stream!.WriteAsync(msg.Buffer.AsMemory(0, CANMsgLength));
    }

    public async Task<CANMessage> ReceiveAsync()
    {
        byte[] buffer = new byte[CANMsgLength];
        await stream!.ReadExactlyAsync(buffer, 0, CANMsgLength);
        return new CANMessage(sender, buffer);
    }
}

