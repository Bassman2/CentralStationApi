using CentralStationWebApi.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        // stop listening
        listener?.Close();
        listener?.Dispose();
    }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<FileReceivedEventArgs>? FileReceived;

    private async Task SendMessageAsync(CANMessage message, CancellationToken cancellationToken = default)
    {
        Debug.WriteLine($"Send: {message}");
        //int x = await sender.SendAsync(message.Buffer, 13, host, 15731);
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        int x = await sender.SendAsync(message.Buffer, 13);
    }

    private string fileName;
    private uint streamLength;
    private List<byte> stream;
    private string streamText;

    private async Task ReceiveAsync()
    {         
        try
        {
            while (true)
            {
                var result = await listener.ReceiveAsync();
                var msg = new CANMessage(result);
                Debug.WriteLine($"Received: {msg}");
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));

                if (msg.Command == Command.ConfigDataStream)
                {
                    if (msg.DataLength != 8)
                    {
                        stream = [];
                        streamLength = msg.UInt0;
                    }
                    else
                    {
                        stream.AddRange(msg.Data);
                        Debug.WriteLine($"Streamed {stream.Count} / {streamLength} bytes");
                        if (stream.Count >= streamLength)
                        {
                            using var file = File.Create("File.zlib");
                            file.Write(stream.ToArray(), 0, (int)streamLength);
                            file.Close();
                                
                            // Decompress zlib-compressed data
                            using var compressed = new MemoryStream(stream.ToArray(), 4, (int)streamLength - 4);
                            //using var zls = new Ionic.Zlib.ZlibStream(compressed, Ionic.Zlib.CompressionMode.Decompress);
                            using var zls = new ZLibStream(compressed, CompressionMode.Decompress);
                            using var outMs = new MemoryStream();
                            await zls.CopyToAsync(outMs);
                            var decompressedBytes = outMs.ToArray();

                            // Convert to string (assume UTF8); adjust if another encoding is used
                            streamText = Encoding.UTF8.GetString(decompressedBytes);

                            FileReceived?.Invoke(this, new FileReceivedEventArgs(fileName, streamText));
                            //autoResetEventConfigDataStream.Set();
                        }
                    }
                    // handle system command response if needed
                }

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
        var message = new SystemMessage(SystemCommand.Stop, 0);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemGoAsync(CancellationToken cancellationToken = default)                            
    {
        var message = new SystemMessage(SystemCommand.Go, 0);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<bool> SystemHaltAsync(CancellationToken cancellationToken = default)
    {
        var message = new SystemMessage(SystemCommand.Halt, 0);

        await SendMessageAsync(message, cancellationToken);

        return true;
    }

    public async Task<string> ConfigDataLocoInfo(CancellationToken cancellationToken = default)
    {
        var message = new ConfigDataMessage("lokinfo");

        await SendMessageAsync(message, cancellationToken);

        return "lokinfo";
    }

    private AutoResetEvent autoResetEventConfigDataStream = new AutoResetEvent(false);
    
    public async Task<string> ConfigDataLocos(CancellationToken cancellationToken = default)
    {
        var message = new ConfigDataMessage("loks");

        await SendMessageAsync(message, cancellationToken);

        //autoResetEventConfigDataStream.WaitOne();
        return streamText;
    }

}


