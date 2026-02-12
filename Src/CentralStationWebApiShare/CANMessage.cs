using System.Net;

namespace CentralStationWebApi;

public class CanMessage 
{
    private const int DLC = 4;      // data length index
    private const int DATA = 5;     // data start index
    private const int SUBC = 9;     // sub command index

    protected readonly byte[] buffer = new byte[13];

    public byte[] Buffer => buffer;

    
    private static readonly string local = System.Net.Dns.GetHostName();

    public  string Sender { get; }

    public DateTime Timestamp { get; }

    #region Send

    public CanMessage(Priority priority, Command command, ushort hash, bool response = false)
    {
        Sender = local;
        Timestamp = DateTime.Now;

        uint header = 0;
        SetBits(ref header, 25, 4, (uint)priority);
        SetBits(ref header, 17, 8, (uint)command);
        SetBits(ref header, 16, 1, response ? 1u : 0u);
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

    public CanMessage AddByte(byte? value)
    {
        if (value is null) return this;

        buffer[DATA + DataLength] = (byte)value;
        DataLength += 1;
        return this;
    }

    public CanMessage AddUInt16(ushort? value)
    {
        if (value is null) return this;

        byte[] data = new byte[sizeof(ushort)];
        Array.Copy(BitConverter.GetBytes((ushort)value), 0, data, 0, sizeof(ushort));
        Array.Reverse(data);
        Array.Copy(data, 0, buffer, DATA + DataLength, sizeof(ushort));
        DataLength += sizeof(ushort);
        return this;
    }

    public CanMessage AddUInt32(uint? value)
    {
        if (value is null) return this;

        byte[] data = new byte[sizeof(uint)];
        Array.Copy(BitConverter.GetBytes((uint)value), 0, data, 0, sizeof(uint));
        Array.Reverse(data);
        Array.Copy(data, 0, buffer, DATA + DataLength, sizeof(uint));
        DataLength += sizeof(uint);
        return this;
    }

    public CanMessage AddString(string value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(DataLength, 0, "Must be first and only AddXxx method");

        byte[] mem = Encoding.ASCII.GetBytes(value);
        Array.Clear(buffer, DATA, 8);
        Array.Copy(mem, 0, buffer, DATA, Math.Min(8, mem.Length));
        DataLength += 8;
        return this;
    }

    public CanMessage AddSubCommand(SubCommand subCommand)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(DataLength, 4, "Must be after DeviceId");

        return AddByte((byte)subCommand);
    }

    #endregion

    #region Receive
    
    public CanMessage(string sender, byte[] data)
    {
        Sender = sender;
        Timestamp = DateTime.Now;
        Array.Copy(data, Buffer, 13);

        // add to DeviceCache & HashCache
        if (Command == Command.SoftwareVersion && DataLength == 8)
        {
            DeviceCache.AddDevice(DeviceId, GetDataUShort(6));
            HashCache.AddHash(Hash, GetDataUShort(6));
        }
    }

    public Priority Priority => (Priority)GetBits(GetHeader(), 25, 4);

    public Command Command => (Command)GetBits(GetHeader(), 17, 8);

    public bool IsResponse => GetBits(GetHeader(), 16, 1) == 1;

    public ushort Hash => (ushort)GetBits(GetHeader(), 0, 16);

    public string Binary => $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} " + string.Join(" ", buffer[5..(5 + Math.Min(buffer[4], (byte)8))].Select(b => b.ToString("X2")));

    
    public uint GetHeader()
    {
        byte[] mem = new byte[sizeof(uint)];
        Array.Copy(buffer, 0, mem, 0, 4);
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    public byte GetDataByte(int index) => buffer[index + DATA];

    public ushort GetDataUShort(int index)
    {
        byte[] mem = new byte[sizeof(short)];
        Array.Copy(buffer, index + DATA, mem, 0, sizeof(short));
        Array.Reverse(mem);
        return BitConverter.ToUInt16(mem, 0);
    }

    public uint GetDataUInt(int index)
    {
        byte[] mem = new byte[sizeof(uint)];
        Array.Copy(buffer, index + DATA, mem, 0, sizeof(uint));
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    public string GetDataString(int index = 0, int length = 8)
    {
        return Encoding.ASCII.GetString(buffer, index + DATA, length).Trim((char)0);
    }

    public byte[] GetData(int index = 0, int length = 8)
    {
        byte[] mem = new byte[length];
        Array.Copy(buffer, index + DATA, mem, 0, length);
        return mem;
    }

    private const uint responseFlag = 0x00010000;
    public bool IsResponseMsgFrom(CanMessage res)
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

    public DirectionChange DirectionChange => (DirectionChange)GetDataByte(4);

    public uint DeviceId => GetDataUInt(0);

    public string DeviceName => $"\"{DeviceCache.DeviceName(DeviceId)}\"";

    #region Description

    public string Description => 
        Command switch
        {
            Command.SystemCommand =>
                SubCommand switch
                {
                    SubCommand.Stop =>                      /* */ $"System Stop - DeviceId: {DeviceName}",
                    SubCommand.Go =>                        /* */ $"System Go - DeviceId: {DeviceName}",
                    SubCommand.Halt =>                      /* */ $"System Halt - DeviceId: {DeviceName}",
                    SubCommand.LocoHalt =>                  /* */ $"System Loco Halt - DeviceId: {DeviceName}",
                    SubCommand.LocoCycleStop =>             /* */ $"System Loco Cycle Stop - DeviceId: {DeviceName}",
                    SubCommand.LocoDataProtocol =>          /* */ $"System Loco Buffer Protocol - DeviceId: {DeviceName}",
                    SubCommand.SwitchingTime =>             /* */ $"System Switching Time - DeviceId: {DeviceName}",
                    SubCommand.FastRead =>                  /* */ $"System Fast Read - DeviceId: {DeviceName}",
                    SubCommand.TrackProtocol =>             /* */ $"System Track Protocol - DeviceId: {DeviceName}",
                    SubCommand.NewRegistrationCounter =>    /* */ $"System New Registration Counter - DeviceId: {DeviceName}",
                    SubCommand.Overload =>                  /* */ $"System Overload - DeviceId: {DeviceName}",
                    SubCommand.Status =>                    /* */ $"System Status - DeviceId: {DeviceName} Channel: {GetDataByte(5)}" + (DataLength == 7 ? $" Value: {GetDataByte(6)}" : "") + (DataLength == 8 ? $" Value: {GetDataUShort(6)}" : ""),
                    SubCommand.Identifier =>                /* */ $"System Identifier - DeviceId: {DeviceName}",
                    SubCommand.Unknown20 =>                 /* */ $"System Unknown20 - DeviceId: {DeviceName}",
                    SubCommand.MfxSeek =>                   /* */ $"System Mfx Seek - DeviceId: {DeviceName}",
                    SubCommand.Reset =>                     /* */ $"System Reset - DeviceId: {DeviceName}",
                    _ => $"Unknown System Command {buffer[SUBC]:X2}"
                },


            Command.Discovery => "Discovery",
            Command.Bind => "Bind",
            Command.Verify => "Verify",
            Command.LocoVelocity =>
                DataLength switch
                {
                    4 => $"Loco Speed - Loco: {DeviceName}",
                    6 => $"Loco Speed - Loco: {DeviceName} Speed: {GetDataUShort(4)}",
                    _ => "Loco Speed unknown data size"
                },
            Command.LocoDirection => 
                DataLength switch
                {
                    4 => $"Loco Direction - Loco: {DeviceName}",
                    5 => $"Loco Direction - Loco: {DeviceName} Direction: {DirectionChange}",
                    _ => "Loco Direction unknown data size"
                },
            Command.LocoFunction =>
                DataLength switch
                {
                    5 => $"Loco Function - Loco: {DeviceName} Function: {GetDataByte(4)}",
                    6 => $"Loco Function - Loco: {DeviceName} Function: {GetDataByte(4)} Value: {GetDataByte(5)}",
                    8 => $"Loco Function - Loco: {DeviceName} Function: {GetDataByte(4)} Value: {GetDataByte(5)} Function value: {GetDataUShort(6)}",
                    _ => "Loco Function unknown data size"
                },
            Command.ReadConfig => "Read Config",
            Command.WriteConfig => "Write Config",
            Command.SwitchAccessories => "Switch Accessories",
            Command.S88Polling => "S88 Polling",
            Command.S88Event => 
                DataLength switch
                {
                    4 => $"S88 Event - DeviceId ID: {GetDataUShort(0)} Contact ID: {GetDataUShort(2)}",
                    5 => $"S88 Event - DeviceId ID: {GetDataUShort(0)} Contact ID: {GetDataUShort(2)} Parameter: {GetDataByte(4)}",
                    8 => $"S88 Event - DeviceId ID: {GetDataUShort(0)} Contact ID: {GetDataUShort(2)} Old: {GetDataByte(4)} New: {GetDataByte(5)} Time: {GetDataUShort(6)}",
                    _ => "S88 Event unknown data size"
                },
            Command.SX1Event => "SX1 Event",
            Command.SoftwareVersion => DataLength == 0 ? "Software Version - Request" :  $"Software Version - DeviceId: {DeviceId:X4} Version: {GetDataByte(4)}.{GetDataByte(5)} DeviceType: {(DeviceType)GetDataUShort(6)} {GetDataUShort(6):X2} # Hash {CentralStation.DeviceId2Hash(DeviceId):X2} #",
            Command.UpdateOffer => "Update Offer",
            Command.ReadConfigData => "Read Config Buffer",
            Command.BootloaderCANBound => "Bootloader CAN Bound",
            Command.BootloaderRailBound => "Bootloader Rail Bound",
            Command.StatusData =>
                DataLength switch
                {
                    5 => $"DeviceId: {DeviceName} Index: {GetDataByte(4)}",
                    6 => $"DeviceId: {DeviceName} Index: {GetDataByte(4)} Num: {GetDataByte(5)}",
                    8 => $"{GetDataString()}",
                    _ => "Unknown Length"


                },
            Command.ConfigDataRequest => $"Config Buffer - Filename: {GetDataString()}",
            Command.ConfigDataStream =>
                DataLength switch
                {
                    6 => $"Config Buffer Stream - Start Response Length {DeviceId}",
                    7 => $"Config Buffer Stream - Start Broadcast Length {DeviceId}",
                    8 => "Config Buffer Stream - Buffer",
                    _ => "Config Buffer Stream - Break"
                },
            Command.DataStream6021Adapter => "Buffer Stream 6021 Adapter",
            Command.AutomaticTransmission => "Automatic Transmission",
            _ => $"Unknown Command 0x{((byte)Command):X2}"
        };

    public override string ToString()
    {
        return $"{GetHeader():X8} {DataLength:X2} {GetDataByte(0):X2} {GetDataByte(1):X2} {GetDataByte(2):X2} {GetDataByte(3):X2} {GetDataByte(4):X2} {GetDataByte(5):X2} {GetDataByte(6):X2} {GetDataByte(7):X2} - " +
            $"Prio.: {Priority} Command: {Command} IsResp: {IsResponse} Hash: {Hash:X4}";
    }

    public string ToTrace()
    {
        string timestamp = Timestamp.ToString("HH:mm:ss.ffff");
        string sender = Dns.GetHostEntry(Sender)?.HostName.Split('.')[0] ?? Sender;
        string data = $"{GetHeader():X8} {DataLength:X} {GetDataByte(0):X2} {GetDataByte(1):X2} {GetDataByte(2):X2} {GetDataByte(3):X2} {GetDataByte(4):X2} {GetDataByte(5):X2} {GetDataByte(6):X2} {GetDataByte(7):X2}";
        string sendReq = IsResponse ? "<--" : "-->";
        string header = $"{Priority,-6} {Command,-20} {sendReq} {HashCache.GetHash(Hash),-20} {Hash:X4}";
        string description = Description;
        return $"{timestamp} {sender,-12} {data} {header} {description}";
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
