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
        client = new();
        //Socket socket = client.Client;
        client.ReceiveTimeout = 5 * 60 * 1000;
    }

    public void Connect(string host)
    {
        while (!client.Connected)
        {
            try
            {
                client.Connect(host, TCPPort);
            }
            catch { }
        }
        //if (!client.ConnectAsync(host, TCPPort).Wait(new TimeSpan(0, 5, 0)))
        //{
        //    throw new SocketException(10060, "Connect timeout");
        //}

        stream = client.GetStream();

        sender = Dns.GetHostEntry(host).HostName.Split('.')[0];
    }

    public bool IsConnected => client.Connected;

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
    
    public void Send(CanMessage msg)
    {
        stream!.Write(msg.Buffer, 0, CANMsgLength);
    }

    public async Task SendAsync(CanMessage msg)
    {
        await stream!.WriteAsync(msg.Buffer.AsMemory(0, CANMsgLength));
    }

    public async Task<CanMessage> ReceiveAsync()
    {
        byte[] buffer = new byte[CANMsgLength];
        await stream!.ReadExactlyAsync(buffer, 0, CANMsgLength);
        return new CanMessage(sender, buffer);
    }
}

