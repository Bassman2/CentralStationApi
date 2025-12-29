using CentralStationWebApi.Model;

namespace CentralStationWebApi;

partial class CentralStation
{
    #region 2 System Commands

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
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash, 8);
        message.SetData($"gbs-{page}");
        SendMessage(message);
    }

    #endregion

    #region 9 Automation

    public void AutomaticTransmission(ushort deviceExpert, ushort automaticFunction, byte position, byte parameter)
    {
        var message = new CANMessage(Priority.Proirity1, Command.AutomaticTransmission, hash, 6);
        message.SetData((ushort)deviceExpert, 5);
        message.SetData((ushort)automaticFunction, 7);
        message.SetData((byte)position, 9);
        message.SetData((byte)parameter, 10);
        SendMessage(message);
    }

    #endregion
}
