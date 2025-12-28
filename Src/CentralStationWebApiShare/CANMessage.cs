namespace CentralStationWebApi;

public class CANMessage : MessageBuffer
{
    
    public CANMessage(Priority priority, Command command, uint hash, byte dataLength = 0)
    {
        SetHeader(priority, command, hash, dataLength);
    }

    public CANMessage(UdpReceiveResult udpReceiveResult)
    {
        Sender = udpReceiveResult.RemoteEndPoint.Address.ToString();
        Array.Copy(udpReceiveResult.Buffer, Buffer, 13);
    }
      

    public DateTime Timestamp { get; } = DateTime.Now;

    public string Sender { get; } = "";

    //public Priority Priority => (Priority)GetBits(GetHeader(), 25, 4);

    //public Command Command => (Command)GetBits(GetHeader(), 17, 8);

    //public bool IsResponse => GetBits(GetDataUInt(0), 16, 1) == 1;

    //public ushort Hash => (ushort)GetBits(GetHeader(), 0, 16);

    //public string Binary => $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2} {buffer[4]:X} " + string.Join(" ", buffer[5..(5 + Math.Min(buffer[4], (byte)8))].Select(b => b.ToString("X2")));

    //public string Filename => Encoding.ASCII.GetString(buffer, 5, buffer[4]);
    //public int DataLength => buffer[4];

    //public byte[] Data
    //{
    //    get
    //    {
    //        byte[] data = new byte[DataLength];
    //        Array.Copy(buffer, 5, data, 0, DataLength);
    //        return data;
    //    }
    //}

    public SystemCommand SystemCommand
    {
        get => (SystemCommand)GetDataByte(9);
        set => SetData((byte)value, 9);
    }

    public uint Device => GetDataUInt(5);

    public string Description => 
        Command switch
        {
            Command.SystemCommand =>
                SystemCommand switch
                {
                    SystemCommand.Stop => $"System Stop - Device: {Device}",
                    SystemCommand.Go => $"System Go - Device: {Device}",
                    SystemCommand.Halt => $"System Halt - Device: {Device}",
                    SystemCommand.LocoHalt => $"System Loco Halt - Device: {Device}",
                    SystemCommand.LocoCycleStop => $"Loco Cycle Stop - Device: {Device}",
                    SystemCommand.LocoDataProtocol => $"Loco Buffer Protocol - Device: {Device}",
                    SystemCommand.SwitchingTime => $"Switching Time - Device: {Device}",
                    SystemCommand.FastRead => $"Fast Read - Device: {Device}",
                    SystemCommand.TrackProtocol => $"Track Protocol - Device: {Device}",
                    SystemCommand.NewRegistrationCounter => $"New Registration Counter - Device: {Device}",
                    SystemCommand.Overload => $"Overload - Device: {Device}",
                    SystemCommand.Status => $"Status - Device: {Device}",
                    SystemCommand.Identifier => $"Identifier - Device: {Device}",
                    SystemCommand.MfxSeek => $"Mfx Seek - Device: {Device}",
                    SystemCommand.Reset => $"System Reset - Device: {Device}",
                    _ => $"Unknown System Command 0x{GetDataByte(9):X2}" 
                },


            Command.Discovery => "Discovery",
            Command.Bind => "Bind",
            Command.Verify => "Verify",
            Command.LocoSpeed =>
                DataLength switch
                {
                    4 => $"Loco Speed - Loco: {Device}",
                    6 => $"Loco Speed - Loco: {Device} Speed: {GetDataUShort(7)}",
                    _ => "Loco Speed unknown data size"
                },
            Command.LocoDirection => 
                DataLength switch
                {
                    4 => $"Loco Direction - Loco: {Device}",
                    5 => $"Loco Direction - Loco: {Device} Direction: " +
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

    //private void SetHeader(Priority priority, Command command, uint hash)
    //{
    //    //uint messageId =
    //    //    ((((uint)priority) & 0x000f) << 25) +       // Prio
    //    //    ((((uint)command) & 0x00ff) << 17) +        // Command
    //    //    ((((uint)0) & 0x0001) << 16) +              // Resp.
    //    //    ((((uint)hash) & 0xffff));            // Command

    //    uint messageId = 0;
    //    SetBits(ref messageId, 25, 4, (uint)priority);
    //    SetBits(ref messageId, 17, 8, (uint)command);
    //    SetBits(ref messageId, 16, 1, 0u);
    //    SetBits(ref messageId, 0, 16, hash);
        
    //    var msgId = BitConverter.GetBytes(messageId);
    //    Array.Reverse(msgId);
    //    Array.Copy(msgId, 0, buffer, 0, 4);
    //}

    //private uint GetHeader()
    //{
    //    var msgId = new byte[4];
    //    Array.Copy(buffer, 0, msgId, 0, 4);
    //    Array.Reverse(msgId);
    //    return BitConverter.ToUInt32(msgId, 0);
    //}

    //private void SetData(byte[] bytes, int length)
    //{
    //    // set DLC
    //    buffer[4] = (byte)length;       
    //    // set data bytes 0 - 7
    //    Array.Copy(bytes, 0, buffer, 5, length);
    //}



    //public uint UInt0
    //{
    //    get
    //    {
    //        byte[] mem = new byte[4];
    //        Array.Copy(buffer, 5, mem, 0, 4);
    //        Array.Reverse(mem);
    //        return BitConverter.ToUInt32(mem, 0);
    //    }
    //}

        
    //private uint UInt01
    //{
    //    get
    //    {
    //        byte[] mem = new byte[4];
    //        Array.Copy(buffer, 9, mem, 0, 4);
    //        Array.Reverse(mem);
    //        return BitConverter.ToUInt32(mem, 0);
    //    }
    //}

    //private ushort UShort0
    //{
    //    get
    //    {
    //        byte[] mem = new byte[2];
    //        Array.Copy(buffer, 5, mem, 0, 2);
    //        Array.Reverse(mem);
    //        return BitConverter.ToUInt16(mem, 0);
    //    }
    //}

    //private ushort UShort1
    //{ 
    //    get
    //    {
    //        byte[] mem = new byte[2];
    //        Array.Copy(buffer, 7, mem, 0, 2);
    //        Array.Reverse(mem);
    //        return BitConverter.ToUInt16(mem, 0);
    //    }
    //}

    //private ushort UShort2
    //{
    //    get
    //    {
    //        byte[] mem = new byte[2];
    //        Array.Copy(buffer, 9, mem, 0, 2);
    //        Array.Reverse(mem);
    //        return BitConverter.ToUInt16(mem, 0);
    //    }
    //}

    //private ushort UShort3
    //{
    //    get
    //    {
    //        byte[] mem = new byte[2];
    //        Array.Copy(buffer, 11, mem, 0, 2);
    //        Array.Reverse(mem);
    //        return BitConverter.ToUInt16(mem, 0);
    //    }
    //}

    //private static void SetBits(ref uint value, int position, int length, uint bits)
    //{
    //    uint mask = ((1u << length) - 1u) << position;
    //    value = (value & ~mask) | ((bits << position) & mask);
    //}   

    //private static uint GetBits(uint value, int position, int length)
    //{
    //    uint mask = (1u << length) - 1u;
    //    return (value >> position) & mask;
    //}
}
