namespace CentralStationWebApi;

partial class CentralStation
{
    #region 2 System Commands

    public void SystemStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Stop);
        SendMessage(message);
    }

    public void SystemGo(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Go);
        SendMessage(message);
    }

    public void SystemHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Halt);
        SendMessage(message);
    }

    public void SystemLocoHalt(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoHalt);
        SendMessage(message);
    }

    public void SystemLocoCycleStop(uint device = AllDevices)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoCycleStop);
        SendMessage(message);
    }

    public void SystemLocoDataProtocol(uint device = AllDevices, byte protocoll = 0xff)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoDataProtocol).
            AddByte(protocoll);
        SendMessage(message);
    }

    public void SystemSwitchingTimel(uint device, ushort time)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.SwitchingTime).
            AddUInt16(time);
        SendMessage(message);
    }

    public void SystemFastRead(uint device, ushort mfxSid)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.FastRead).
            AddUInt16(mfxSid);
        SendMessage(message);
    }

    public void SystemTrackProtocol(uint device, byte protocoll)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.TrackProtocol).
            AddByte(protocoll); 
        SendMessage(message);
    }

    public void SystemNewRegistrationCounter(uint device, ushort newRegistrationCounter)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.NewRegistrationCounter).
            AddUInt16(newRegistrationCounter);
        SendMessage(message);
    }

    public void SystemOverload(uint device, byte channel)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Overload).
            AddByte(channel);
        SendMessage(message);
    }

    public void SystemStatus(uint device, byte channel, ushort? value = null)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Status).
            AddByte(channel).
            AddUInt16(value);   // optional
        SendMessage(message);
    }

    public void SystemIdentifier(uint device, byte identifier)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Identifier).
            AddUInt16(identifier);   // optional
        SendMessage(message);
    }

    public void SystemMfxSeek(uint device)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.MfxSeek);
        SendMessage(message);
    }

    public void SystemReset(uint device, byte target)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Reset).
            AddByte(target);
        SendMessage(message);
    }

    #endregion

    #region 3 Administration

    #endregion

    #region 4 Article Commands

    #endregion

    #region 5 Feedback

    #endregion

    #region 6 Other Commands / Sonstige Befehle

    public void RequestParticipants()
    {
        var message = new CANMessage(Priority.Proirity1, Command.SoftwareVersion, hash);
        SendMessage(message);
    }

    #endregion

    #region 7 GUI Information Transfer

    public void RequestConfigDataLocomotives()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("loks");
        SendMessage(message);
    }

    public void RequestConfigDataMagneticItems()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("mags");
        SendMessage(message);
    }
    public void RequestConfigDataRailwayRoute()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("fs");
        SendMessage(message);
    }

    public void RequestConfigDataTrackDiagram()
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("gbs");
        SendMessage(message);
    }

    public void RequestConfigDataTrackDiagramPage(int page)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString($"gbs-{page}");
        SendMessage(message);
    }

    #endregion

    #region 9 Automation

    public void AutomaticTransmission(ushort deviceExpert, ushort automaticFunction, byte position, byte parameter)
    {
        var message = new CANMessage(Priority.Proirity1, Command.AutomaticTransmission, hash).
            AddUInt16(deviceExpert).
            AddUInt16(automaticFunction).
            AddByte(position).
            AddByte(parameter);
        SendMessage(message);
    }

    #endregion
}
