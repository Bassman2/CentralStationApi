using System.Net;

namespace CentralStationWebApi;

/// <summary>
/// Represents a CAN (Controller Area Network) message used for communication with the Märklin Central Station.
/// Provides methods for constructing, sending, receiving, and interpreting CAN bus protocol messages.
/// </summary>
public sealed class CanMessage 
{
    private const int DLC = 4;      // data length index
    private const int DATA = 5;     // data start index
    private const int SUBC = 9;     // sub command index

    private readonly byte[] buffer = new byte[13];

    /// <summary>
    /// Gets the raw message buffer.
    /// </summary>
    public byte[] Buffer => buffer;

    
    private static readonly string local = System.Net.Dns.GetHostName();

    /// <summary>
    /// Gets the sender hostname of this message.
    /// </summary>
    public string Sender { get; }

    /// <summary>
    /// Gets the timestamp when this message was created or received.
    /// </summary>
    public DateTime Timestamp { get; }

    #region Send

    /// <summary>
    /// Initializes a new CAN message for sending to the Central Station.
    /// </summary>
    /// <param name="priority">The message priority level.</param>
    /// <param name="command">The command type.</param>
    /// <param name="hash">The hash value derived from the device ID.</param>
    /// <param name="response">Indicates whether this is a response message. Default is false.</param>
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

    /// <summary>
    /// Gets or sets the length of the data payload in bytes.
    /// </summary>
    public byte DataLength
    {
        get => buffer[DLC];
        set => buffer[DLC] = value;
    }

    /// <summary>
    /// Adds a byte value to the message data payload.
    /// </summary>
    /// <param name="value">The byte value to add, or null to skip.</param>
    /// <returns>This <see cref="CanMessage"/> instance for method chaining.</returns>
    public CanMessage AddByte(byte? value)
    {
        if (value is null) return this;

        buffer[DATA + DataLength] = (byte)value;
        DataLength += 1;
        return this;
    }

    /// <summary>
    /// Adds a 16-bit unsigned integer value to the message data payload in big-endian format.
    /// </summary>
    /// <param name="value">The 16-bit value to add, or null to skip.</param>
    /// <returns>This <see cref="CanMessage"/> instance for method chaining.</returns>
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

    /// <summary>
    /// Adds a 32-bit unsigned integer value to the message data payload in big-endian format.
    /// </summary>
    /// <param name="value">The 32-bit value to add, or null to skip.</param>
    /// <returns>This <see cref="CanMessage"/> instance for method chaining.</returns>
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

    /// <summary>
    /// Adds an ASCII string to the message data payload (up to 8 characters).
    /// Must be the first and only data added to the message.
    /// </summary>
    /// <param name="value">The string to add.</param>
    /// <returns>This <see cref="CanMessage"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if data has already been added to the message.</exception>
    public CanMessage AddString(string value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(DataLength, 0, "Must be first and only AddXxx method");

        byte[] mem = Encoding.ASCII.GetBytes(value);
        Array.Clear(buffer, DATA, 8);
        Array.Copy(mem, 0, buffer, DATA, Math.Min(8, mem.Length));
        DataLength += 8;
        return this;
    }

    /// <summary>
    /// Adds a sub-command byte to the message data payload.
    /// Must be called after adding a device ID (4 bytes).
    /// </summary>
    /// <param name="subCommand">The sub-command to add.</param>
    /// <returns>This <see cref="CanMessage"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if not called after adding a device ID.</exception>
    public CanMessage AddSubCommand(SubCommand subCommand)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(DataLength, 4, "Must be after DeviceId");

        return AddByte((byte)subCommand);
    }

    #endregion

    #region Receive

    /// <summary>
    /// Initializes a new CAN message from received data.
    /// </summary>
    /// <param name="sender">The hostname of the sender.</param>
    /// <param name="data">The raw message data (13 bytes).</param>
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

    /// <summary>
    /// Gets the message priority level.
    /// </summary>
    public Priority Priority => (Priority)GetBits(GetHeader(), 25, 4);

    /// <summary>
    /// Gets the command type of this message.
    /// </summary>
    public Command Command => (Command)GetBits(GetHeader(), 17, 8);

    /// <summary>
    /// Gets a value indicating whether this is a response message.
    /// </summary>
    public bool IsResponse => GetBits(GetHeader(), 16, 1) == 1;

    /// <summary>
    /// Gets the hash value associated with this message.
    /// </summary>
    public ushort Hash => (ushort)GetBits(GetHeader(), 0, 16);

    /// <summary>
    /// Gets the message in binary hexadecimal format for debugging.
    /// </summary>
    public string Binary => $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} " + string.Join(" ", buffer[5..(5 + Math.Min(buffer[4], (byte)8))].Select(b => b.ToString("X2")));

    /// <summary>
    /// Gets the 32-bit header value from the message buffer.
    /// </summary>
    /// <returns>The header as a 32-bit unsigned integer.</returns>
    public uint GetHeader()
    {
        byte[] mem = new byte[sizeof(uint)];
        Array.Copy(buffer, 0, mem, 0, 4);
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    /// <summary>
    /// Gets a byte value from the data payload at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index within the data payload.</param>
    /// <returns>The byte value at the specified index.</returns>
    public byte GetDataByte(int index) => buffer[index + DATA];

    /// <summary>
    /// Gets a 16-bit unsigned integer value from the data payload at the specified index (big-endian).
    /// </summary>
    /// <param name="index">The zero-based index within the data payload.</param>
    /// <returns>The 16-bit value at the specified index.</returns>
    public ushort GetDataUShort(int index)
    {
        byte[] mem = new byte[sizeof(short)];
        Array.Copy(buffer, index + DATA, mem, 0, sizeof(short));
        Array.Reverse(mem);
        return BitConverter.ToUInt16(mem, 0);
    }

    /// <summary>
    /// Gets a 32-bit unsigned integer value from the data payload at the specified index (big-endian).
    /// </summary>
    /// <param name="index">The zero-based index within the data payload.</param>
    /// <returns>The 32-bit value at the specified index.</returns>
    public uint GetDataUInt(int index)
    {
        byte[] mem = new byte[sizeof(uint)];
        Array.Copy(buffer, index + DATA, mem, 0, sizeof(uint));
        Array.Reverse(mem);
        return BitConverter.ToUInt32(mem, 0);
    }

    /// <summary>
    /// Gets an ASCII string from the data payload.
    /// </summary>
    /// <param name="index">The zero-based index within the data payload. Default is 0.</param>
    /// <param name="length">The length of the string in bytes. Default is 8.</param>
    /// <returns>The ASCII string, trimmed of null characters.</returns>
    public string GetDataString(int index = 0, int length = 8)
    {
        return Encoding.ASCII.GetString(buffer, index + DATA, length).Trim((char)0);
    }

    /// <summary>
    /// Gets a byte array from the data payload.
    /// </summary>
    /// <param name="index">The zero-based index within the data payload. Default is 0.</param>
    /// <param name="length">The number of bytes to retrieve. Default is 8.</param>
    /// <returns>A byte array containing the requested data.</returns>
    public byte[] GetData(int index = 0, int length = 8)
    {
        byte[] mem = new byte[length];
        Array.Copy(buffer, index + DATA, mem, 0, length);
        return mem;
    }

    private const uint responseFlag = 0x00010000;

    /// <summary>
    /// Determines whether the specified message is a response to this message.
    /// </summary>
    /// <param name="res">The message to check.</param>
    /// <returns>True if <paramref name="res"/> is a response to this message; otherwise, false.</returns>
    public bool IsResponseMsgFrom(CanMessage res)
    {
        return ((this.GetDataUInt(0) | responseFlag) == res.GetDataUInt(0)) &&
            (this.Command != Command.SystemCommand || this.GetDataByte(9) == res.GetDataByte(9));   // on SubCommand compare sub command
    }

    #endregion

    /// <summary>
    /// Gets or sets the sub-command for system commands.
    /// </summary>
    public SubCommand SubCommand
    {
        get => (SubCommand)buffer[SUBC];
        set => buffer[SUBC] = (byte)value;
    }

    /// <summary>
    /// Gets the direction change value for locomotive direction commands.
    /// </summary>
    public DirectionChange DirectionChange => (DirectionChange)GetDataByte(4);

    /// <summary>
    /// Gets the device ID from the message data payload.
    /// </summary>
    public uint DeviceId => GetDataUInt(0);

    /// <summary>
    /// Gets the device name associated with the device ID, retrieved from the device cache.
    /// </summary>
    public string DeviceName => $"\"{DeviceCache.DeviceName(DeviceId)}\"";

    #region Description

    /// <summary>
    /// Gets a human-readable description of the message content based on the command and data.
    /// </summary>
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

    /// <summary>
    /// Returns a string representation of the message showing the header, data, and command details.
    /// </summary>
    /// <returns>A formatted string representation of the message.</returns>

    public override string ToString()
    {
        return $"{GetHeader():X8} {DataLength:X2} {GetDataByte(0):X2} {GetDataByte(1):X2} {GetDataByte(2):X2} {GetDataByte(3):X2} {GetDataByte(4):X2} {GetDataByte(5):X2} {GetDataByte(6):X2} {GetDataByte(7):X2} - " +
            $"Prio.: {Priority} Command: {Command} IsResp: {IsResponse} Hash: {Hash:X4}";
    }

    /// <summary>
    /// Returns a detailed trace string representation of the message for logging and debugging.
    /// Includes timestamp, sender, raw data, and human-readable description.
    /// </summary>
    /// <returns>A formatted trace string with complete message details.</returns>
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

    /// <summary>
    /// Sets specific bits within a 32-bit unsigned integer value.
    /// </summary>
    /// <param name="value">The value to modify (passed by reference).</param>
    /// <param name="position">The starting bit position (0-31).</param>
    /// <param name="length">The number of bits to set.</param>
    /// <param name="bits">The bit values to set at the specified position.</param>
    private static void SetBits(ref uint value, int position, int length, uint bits)
    {
        uint mask = ((1u << length) - 1u) << position;
        value = (value & ~mask) | ((bits << position) & mask);
    }

    /// <summary>
    /// Extracts specific bits from a 32-bit unsigned integer value.
    /// </summary>
    /// <param name="value">The value to extract bits from.</param>
    /// <param name="position">The starting bit position (0-31).</param>
    /// <param name="length">The number of bits to extract.</param>
    /// <returns>The extracted bit values as an unsigned integer.</returns>

    private static uint GetBits(uint value, int position, int length)
    {
        uint mask = (1u << length) - 1u;
        return (value >> position) & mask;
    }

    #endregion
}
