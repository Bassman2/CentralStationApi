using System.IO.Ports;

namespace CentralStationApi.Internal;

//  CAN 2.0B
// 29-Bit-IDs 
// Baudrate 250 kBaud.

// CAN Mode: Normal
// Frame Type: Extended Frame (CAN2.0B 29-bit ID)
// CAN Baud Rate: 250k
// SYNC_SEG: CAN_SJW_1tq
// BS1: CAN_BS1_6tq
// BS2: CAN_BS2_7tq
// Prescale (Dec): 12


// https://www.waveshare.com/wiki/USB-CAN-B?srsltid=AfmBOoq1ePaZ7Enp9sWvnNCJVRptROrrDNTx7FSYwDnfiY9j88lU6aqt#Driver_Installation
// https://www.waveshare.com/wiki/USB-CAN-A?srsltid=AfmBOoq7ZrcF4TVP25PmkYyEvwLRpUS7c4MgSnb8M0thHWall8IavnV6#Features

// 0 07/04/2026 22:13:47 IRP_MJ_WRITE DOWN  aa 55 02 05 02 00 00 00 00 00 00 00 00 00 01 00 00 00 00 0a  ªU.................. 20 20 COM4  


// AA Cx 11xx xxxx Variable-length communication protocol format
// AA E8
// AA 55 01 01 01   Fixed 20-byte protocol format
// AA 55 02         Setting (for sending and receiving data with a fixed 20-byte protocol)
// AA 55 12         Setting (for sending and receiving data with a variable protocol)



internal sealed class CanHandler : IProtocolHandler, IDisposable
{
    private const int msgLength = 20;
    private const int comBaudrate = 2_000_000;

    private const int canBaudrate = 250_000;

    private SerialPort? serialPort;
    private string sender = "CAN";

    public bool IsConnected => serialPort is not null && serialPort.IsOpen;

    public void Connect(string host)
    {
        sender = host;

        serialPort = new SerialPort(host, comBaudrate, Parity.None, 8, StopBits.One);
        serialPort.DataReceived += SerialPort_DataReceived;
        serialPort.ErrorReceived += SerialPort_ErrorReceived;
        //serialPort.ReadTimeout = 1000 * 60 * 60;
        serialPort.Open();
        //serialPort.DiscardInBuffer();
        //serialPort.DiscardOutBuffer();
        //ConfigureCan();
    }

    private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
       
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

        var buffer = new byte[msgLength];

        while (true)
        {
            int num = serialPort.BytesToRead;

            if (num >= 20)
            {
                Console.WriteLine(num);
                int len = await serialPort!.BaseStream.ReadAsync(buffer, 0, 20);
                
                if (buffer[0] != 0xAA)
                {
                    Console.WriteLine("No AA header");
                }
                if (buffer[1] != 0x55)
                {
                    Console.WriteLine("Not supported format");
                }


                //Console.WriteLine(len);
                //await serialPort!.BaseStream.ReadExactlyAsync(buffer, 0, msgLength);

                string str = buffer.Select(i => i.ToString("X2")).Aggregate("", (a, b) => $"{a} {b}"); // prevent optimization
                Console.WriteLine(str);
            }
            else
            {
                Console.WriteLine($"empty");
                Thread.Sleep(1000);
            }
        }
        return new CanMessage(sender, buffer, 5);
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

    private void ConfigureCan()
    {
        // aa 55 02 05 02 00 00 00 00 00 00 00 00 00 01 00 00 00 00 0a

        var data = new byte[] {
            0xaa,   // Message header
            0x55,   // Message header: CAN Configuration
            0x02,   // Type: 0x02-Setting (for sending and receiving data with a fixed 20-byte protocol)
            0x05,   // CAN baud rate: 0x05 (250kbps)
            0x02,   // Frame type: 0x02 Extended frame
            0x00,   // Filter ID1
            0x00,   // Filter ID2
            0x00,   // Filter ID3
            0x00,   // Filter ID4
            0x00,   // Block ID1
            0x00,   // Block ID2
            0x00,   // Block ID3
            0x00,   // Block ID4
            0x00,   // CAN mode: 0x00-Normal mode
            0x01,   // Auto-retransmit:	0x01—Disable auto-retransmit
            0x00,   // Backup
            0x00,   // Backup
            0x00,   // Backup
            0x00,   // Backup
            0x0a};  // Checksum: The low 8 bits (red part) of the cumulative sum from the frame type to the error codez

        // Checksum:
        // For example, if you send a standard frame ID 0x123 with data of 11 22 33 44 55 66 77 88, the check code is calculated as follows:
        // 0x01 + 0x01 + 0x01 + 0x23 + 0x01 + 0x00 + 0x00 + 0x08 + 0x11 + 0x22 + 0x33 + 0x44 + 0x55 + 0x66 + 0x77 + 0x88 + 0x00 = 0x293
        // The lower 8 bits are 0x93.

        serialPort!.Write(data, 0, data.Length);
    }           
}               


