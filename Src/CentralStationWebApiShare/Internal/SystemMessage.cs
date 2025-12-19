namespace CentralStationWebApi.Internal;

internal class SystemMessage : CANMessage
{
    public SystemMessage(SystemCommand systemCommand, uint device)
        : base(Priority.Proirity1, Command.SystemCommand, CreateData(systemCommand, device))
    {
    }

    private static byte[] CreateData(SystemCommand command, uint device)
    {
        byte[] data;
        switch (command)
        {
        case SystemCommand.SystemStop:
        case SystemCommand.SystemGo:
        case SystemCommand.SystemHalt:

            data = new byte[5];
            Array.Clear(data, 0, 5);
            Array.Copy(BitConverter.GetBytes(device), 0, data, 0, 4);
            data[4] = (byte)command;
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(command), $"Unsupported system command: {command}");
        }
        return data;
    }
}