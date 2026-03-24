using System.IO.Ports;

namespace CentralStationApi.Internal;

//  CAN 2.0B
// 29-Bit-IDs 
// Baudrate 250 kBaud.

// https://www.waveshare.com/wiki/USB-CAN-B?srsltid=AfmBOoq1ePaZ7Enp9sWvnNCJVRptROrrDNTx7FSYwDnfiY9j88lU6aqt#Driver_Installation
// https://www.waveshare.com/wiki/USB-CAN-A?srsltid=AfmBOoq7ZrcF4TVP25PmkYyEvwLRpUS7c4MgSnb8M0thHWall8IavnV6#Features

internal sealed class CanHandler : IProtocolHandler, IDisposable
{
    private const int msgLength = 13;
    private const int canBaudrate = 250000;

    private SerialPort? serialPort;
    private string sender = "CAN";

    public bool IsConnected => serialPort is not null && serialPort.IsOpen;

    public void Connect(string host)
    {
        sender = host; 

        serialPort = new SerialPort(host, canBaudrate);
        serialPort.Open();
    }

    public void Dispose()
    {
        if (serialPort is not null)
        {
            serialPort.Close();
            serialPort.Dispose();
            serialPort = null;
        }   
    }

    public async Task<CanMessage> ReceiveAsync()
    {
        if (serialPort is null || !serialPort.IsOpen) throw new InvalidOperationException("Serial port is not initialized.");

        var buffer = new byte[13];
        await serialPort!.BaseStream.ReadExactlyAsync(buffer, 0, msgLength);
        return new CanMessage(sender, buffer);
    }

    public void Send(CanMessage msg)
    {
        ArgumentNullException.ThrowIfNull(msg, nameof(msg));
        if (serialPort is null || !serialPort.IsOpen) throw new InvalidOperationException("Serial port is not initialized.");
        
        serialPort.Write(msg.Buffer, 0, msgLength);
    }

    public async Task SendAsync(CanMessage msg)
    {
        ArgumentNullException.ThrowIfNull(msg, nameof(msg));
        if (serialPort is null || !serialPort.IsOpen) throw new InvalidOperationException("Serial port is not initialized.");

        await serialPort!.BaseStream.WriteAsync(msg.Buffer, 0, msgLength);
    }
}


