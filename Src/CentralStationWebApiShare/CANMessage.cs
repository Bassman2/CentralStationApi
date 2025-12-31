using System.Net;

namespace CentralStationWebApi;

public class CANMessage 
{
    private const int DLC = 4;      // data length index
    private const int DATA = 5;     // data start index
    private const int SUBC = 9;     // sub command index

    protected readonly byte[] buffer = new byte[13];

    public byte[] Buffer => buffer;

    public DateTime Timestamp { get; } = DateTime.Now;
    public string Sender { get; } = "";


    #region Send

    public CANMessage(Priority priority, Command command, uint hash)
    {
        //SetHeader(priority, command, hash, dataLength);

        uint header = 0;
        SetBits(ref header, 25, 4, (uint)priority);
        SetBits(ref header, 17, 8, (uint)command);
        SetBits(ref header, 16, 1, 0u);
        SetBits(ref header, 0, 16, hash);

        byte[] data = new byte[sizeof(uint)];
        Array.Copy(BitConverter.GetBytes(header), 0, data, 0, sizeof(uint));
        Array.Reverse(data);
        Array.Copy(data, 0, buffer, 0, sizeof(uint));

        //SetData(header, 0);
        DataLength = 0;
    }

    public byte DataLength
    {
        get => buffer[DLC];
        set => buffer[DLC] = value;
    }

    public CANMessage AddByte(byte? value)
    {
        if (value is null) return this;

        buffer[DATA + DataLength] = (byte)value;
        DataLength += 1;
        return this;
    }

    public CANMessage AddUInt16(ushort? value)
    {
        if (value is null) return this;

        byte[] data = new byte[sizeof(ushort)];
        Array.Copy(BitConverter.GetBytes((ushort)value), 0, data, 0, sizeof(ushort));
        Array.Reverse(data);
        Array.Copy(data, 0, buffer, DATA + DataLength, sizeof(ushort));
        DataLength += sizeof(ushort);
        return this;
    }

    public CANMessage AddUInt32(uint? value)
    {
        if (value is null) return this;

        byte[] data = new byte[sizeof(uint)];
        Array.Copy(BitConverter.GetBytes((uint)value), 0, data, 0, sizeof(uint));
        Array.Reverse(data);
        Array.Copy(data, 0, buffer, DATA + DataLength, sizeof(uint));
        DataLength += sizeof(uint);
        return this;
    }

    public CANMessage AddString(string value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(DataLength, 0, "Must be first and only AddXxx method");

        byte[] mem = Encoding.ASCII.GetBytes(value);
        Array.Clear(buffer, DATA, 8);
        Array.Copy(mem, 0, buffer, DATA, Math.Min(8, mem.Length));
        DataLength += 8;
        return this;
    }

    public CANMessage AddSubCommand(SubCommand subCommand)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(DataLength, 4, "Must be after Device");

        return AddByte((byte)subCommand);
    }

    #endregion

    #region Receive

    public CANMessage(UdpReceiveResult udpReceiveResult)
    {
        Sender = udpReceiveResult.RemoteEndPoint.Address.ToString();
        Array.Copy(udpReceiveResult.Buffer, Buffer, 13);
    }

    public Priority Priority => (Priority)GetBits(GetDataUInt(0), 25, 4);

    public Command Command => (Command)GetBits(GetDataUInt(0), 17, 8);

    public bool IsResponse => GetBits(GetDataUInt(0), 16, 1) == 1;

    public ushort Hash => (ushort)GetBits(GetDataUInt(0), 0, 16);

    public string Binary => $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} " + string.Join(" ", buffer[5..(5 + Math.Min(buffer[4], (byte)8))].Select(b => b.ToString("X2")));

    public byte GetDataByte(int index) => buffer[index];

    public ushort GetDataUShort(int index)
    {
        byte[] mem = new byte[2];
        Array.Copy(buffer, index, mem, 0, 2);
        Array.Reverse(mem);
        return BitConverter.ToUInt16(mem, 0);
    }

    public uint GetDataUInt(int index)
    {
        byte[] mem = new byte[4];
        Array.Copy(buffer, index, mem, 0, 4);
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    public string GetDataString(int index = 5, int length = 8)
    {
        return Encoding.ASCII.GetString(buffer, index, length);
    }

    public byte[] GetData(int index = 5, int length = 8)
    {
        byte[] mem = new byte[length];
        Array.Copy(buffer, index, mem, 0, length);
        return mem;
    }

    private const uint responseFlag = 0x00010000;
    public bool IsResponseMsgFrom(CANMessage res)
    {
        return ((this.GetDataUInt(0) | responseFlag) == res.GetDataUInt(0)) &&
            (this.Command != Command.SystemCommand || this.GetDataByte(9) == res.GetDataByte(9));   // on SubCommand compare sub command


    }

    #endregion


    public SubCommand SubCommand
    {
        get => (SubCommand)buffer[SUBC];
        set => buffer[SUBC] = (byte)value;
    }

    public uint Device => GetDataUInt(5);

    #region Description

    public string Description => 
        Command switch
        {
            Command.SystemCommand =>
                SubCommand switch
                {
                    SubCommand.Stop => $"System Stop - Device: {Device:X4}",
                    SubCommand.Go => $"System Go - Device: {Device:X4}",
                    SubCommand.Halt => $"System Halt - Device: {Device:X4}",
                    SubCommand.LocoHalt => $"System Loco Halt - Device: {Device:X4}",
                    SubCommand.LocoCycleStop => $"Loco Cycle Stop - Device: {Device:X4}",
                    SubCommand.LocoDataProtocol => $"Loco Buffer Protocol - Device: {Device:X4}",
                    SubCommand.SwitchingTime => $"Switching Time - Device: {Device:X4}",
                    SubCommand.FastRead => $"Fast Read - Device: {Device:X4}",
                    SubCommand.TrackProtocol => $"Track Protocol - Device: {Device:X4}",
                    SubCommand.NewRegistrationCounter => $"New Registration Counter - Device: {Device:X4}",
                    SubCommand.Overload => $"Overload - Device: {Device:X4}",
                    SubCommand.Status => $"Status - Device: {Device:X4}",
                    SubCommand.Identifier => $"Identifier - Device: {Device:X4}",
                    SubCommand.MfxSeek => $"Mfx Seek - Device: {Device:X4}",
                    SubCommand.Reset => $"System Reset - Device: {Device:X4}",
                    _ => $"Unknown System Command 0x{GetDataByte(9):X2}" 
                },


            Command.Discovery => "Discovery",
            Command.Bind => "Bind",
            Command.Verify => "Verify",
            Command.LocoVelocity =>
                DataLength switch
                {
                    4 => $"Loco Speed - Loco: {Device:X4}",
                    6 => $"Loco Speed - Loco: {Device:X4} Speed: {GetDataUShort(7)}",
                    _ => "Loco Speed unknown data size"
                },
            Command.LocoDirection => 
                DataLength switch
                {
                    4 => $"Loco Direction - Loco: {Device:X4}",
                    5 => $"Loco Direction - Loco: {Device:X4} Direction: " +
                        GetDataByte(10) switch
                        {
                            0x00 => "Stay",
                            0x01 => "Forward",
                            0x02 => "Backward",
                            0x03 => "Switch",
                            _ => $"Unknown Direction 0x{GetDataByte(10):X2}"
                        },
                    _ => "Loco Direction unknown data size"
                },
            Command.LocoFunction => "Loco Function",
            Command.ReadConfig => "Read Config",
            Command.WriteConfig => "Write Config",
            Command.SwitchAccessories => "Switch Accessories",
            Command.S88Polling => "S88 Polling",
            Command.S88Event => "S88 Event",
            Command.SX1Event => "SX1 Event",
            Command.SoftwareVersion => IsResponse ? $"Software Version - Sender: {Device} Version: {GetDataUShort(7)} Device: 0x{GetDataByte(11):X2} 0x{GetDataByte(12):X2}" : "Software Version",
            Command.UpdateOffer => "Update Offer",
            Command.ReadConfigData => "Read Config Buffer",
            Command.BootloaderCANBound => "Bootloader CAN Bound",
            Command.BootloaderRailBound => "Bootloader Rail Bound",
            Command.StatusData => "Status Buffer",
            Command.RequestConfigData => $"Config Buffer - Filename: {GetDataString()}",
            Command.ConfigDataStream =>
                DataLength switch
                {
                    6 => $"Config Buffer Stream - Start Response Length {Device}",
                    7 => $"Config Buffer Stream - Start Broadcast Length {Device}",
                    8 => "Config Buffer Stream - Buffer",
                    _ => "Config Buffer Stream - Break"
                },
            Command.DataStream6021Adapter => "Buffer Stream 6021 Adapter",
            Command.AutomaticTransmission => "Automatic Transmission",
            _ => $"Unknown Command 0x{((byte)Command):X2}"
        };

    public override string ToString()
    {
        return $"{GetDataByte(0):X2}{GetDataByte(1):X2}{GetDataByte(2):X2}{GetDataByte(3):X2} {GetDataByte(4):X} {GetDataByte(5):X2} {GetDataByte(6):X2} {GetDataByte(7):X2} {GetDataByte(8):X2} {GetDataByte(9):X2} {GetDataByte(10):X2} {GetDataByte(11):X2} {GetDataByte(12):X2} - " +
            $"Prio.: {Priority} Command: {Command} IsResp: {IsResponse} Hash: {Hash:X4}";
    }

    public string ToTrace()
    {
        string timestamp = Timestamp.ToString("HH:mm:ss.ffff");
        string sender = Dns.GetHostEntry(Sender)?.HostName.Split('.')[0] ?? Sender;
        string data = $"{GetDataByte(0):X2}{GetDataByte(1):X2}{GetDataByte(2):X2}{GetDataByte(3):X2} {GetDataByte(4):X} {GetDataByte(5):X2} {GetDataByte(6):X2} {GetDataByte(7):X2} {GetDataByte(8):X2} {GetDataByte(9):X2} {GetDataByte(10):X2} {GetDataByte(11):X2} {GetDataByte(12):X2}";
        string sendReq = IsResponse ? "<--" : "-->";
        string header = $"{Priority} {Command.ToString().PadRight(20)} {sendReq} {Hash:X4}";
        string description = Description;
        return $"{timestamp} {sender.PadRight(12)} {data} {header} {description}";
    }

    #endregion

        #region Bits

    protected static void SetBits(ref uint value, int position, int length, uint bits)
    {
        uint mask = ((1u << length) - 1u) << position;
        value = (value & ~mask) | ((bits << position) & mask);
    }

    protected static uint GetBits(uint value, int position, int length)
    {
        uint mask = (1u << length) - 1u;
        return (value >> position) & mask;
    }

    #endregion
}
