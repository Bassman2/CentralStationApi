//namespace CentralStationWebApi.Internal;

//internal class SystemMessage : CANMessage
//{
//    public SystemMessage(SystemCommand systemCommand, uint device)
//        : base(Priority.Proirity1, Command.SystemCommand, 0x4711, CreateData(systemCommand, device))
//    { }

//    public SystemMessage(SystemCommand systemCommand, uint device, byte value)
//        : base(Priority.Proirity1, Command.SystemCommand, 0x4711, CreateData(systemCommand, device, value))
//    { }

//    private static byte[] CreateData(SystemCommand command, uint device)
//    {
//        byte[] data;
//        switch (command)
//        {
//        case SystemCommand.Stop:
//        case SystemCommand.Go:
//        case SystemCommand.Halt:
//        case SystemCommand.LocoHalt:
//        case SystemCommand.LocoCycleStop:
//            data = new byte[5];
//            Array.Clear(data, 0, 5);
//            Array.Copy(BitConverter.GetBytes(device), 0, data, 0, 4);
//            data[4] = (byte)command;
//            break;
//        default:
//            throw new ArgumentOutOfRangeException(nameof(command), $"Unsupported system command: {command}");
//        }
//        return data;
//    }

//    private static byte[] CreateData(SystemCommand command, uint device, byte value)
//    {
//        byte[] data;
//        switch (command)
//        {
//        case SystemCommand.LocoDataProtocol:
//            data = new byte[6];
//            Array.Clear(data, 0, 5);
//            Array.Copy(BitConverter.GetBytes(device), 0, data, 0, 4);
//            data[4] = (byte)command;
//            data[5] = value;
//            break;
//        default:
//            throw new ArgumentOutOfRangeException(nameof(command), $"Unsupported system command: {command}");
//        }
//        return data;
//    }
//}