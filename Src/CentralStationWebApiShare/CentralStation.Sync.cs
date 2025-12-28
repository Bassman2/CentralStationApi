namespace CentralStationWebApi;

partial class CentralStation
{
    #region System Commands

    public void SystemStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Stop);
        SendMessage(message);
    }

    public void SystemGo(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Go);
        SendMessage(message);
    }

    public void SystemHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Halt);
        SendMessage(message);
    }

    public void SystemLocoHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoHalt);
        SendMessage(message);
    }

    public void SystemLocoCycleStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoCycleStop);
        SendMessage(message);
    }

    public void SystemLocoDataProtocol(uint device = AllDevices, byte protocoll = 0xff)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoDataProtocol);
        SendMessage(message);
    }

    #endregion

    #region 6 Other Commands / Sonstige Befehle

    public void RequestParticipants()
    {
        var message = new CANMessage(Priority.Proirity1, Command.SoftwareVersion, hash);
        SendMessage(message);
    }

    #endregion

    #region GUI Information Transfer / GUI Informationsübertragung

    public async Task<string> ConfigDataLocomotivesInfo(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("lokinfo");
        await SendMessageAsync(message, cancellationToken);

        return "lokinfo";
    }

    public void RequestConfigDataLocomotives()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("loks");
        SendMessage(message);
    }

    public void RequestConfigDataMagneticItems()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("mags");
        SendMessage(message);
    }
    public void RequestConfigDataRailwayRoute()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("fs");
        SendMessage(message);
    }

    public void RequestConfigDataTrackDiagram()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("gbs");
        SendMessage(message);
    }

    public void RequestConfigDataTrackDiagramPage(int page)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData($"gbs-{page}");
        SendMessage(message);
    }

    #endregion
}
