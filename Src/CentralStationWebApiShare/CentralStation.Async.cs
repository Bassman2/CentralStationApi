namespace CentralStationWebApi;

partial class CentralStation
{
    #region System Commands


    public async Task<CANMessage> SystemStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Stop);
        return await SendMessageAsync(message, cancellationToken);
    }



    public async Task<CANMessage> SystemGoAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Go);
        return await SendMessageAsync(message, cancellationToken);
    }


    public async Task<CANMessage> SystemHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Halt);
        return await SendMessageAsync(message, cancellationToken);
    }



    public async Task<CANMessage> SystemLocoHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoHalt);
        return await SendMessageAsync(message, cancellationToken);
    }



    public async Task<CANMessage> SystemLocoCycleStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoCycleStop);
        return await SendMessageAsync(message, cancellationToken);
    }



    public async Task<CANMessage> SystemLocoDataProtocolAsync(uint device = AllDevices, byte protocoll = 0xff, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoDataProtocol);
        return await SendMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemSwitchingTimeAsync(uint device, ushort time, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.LocoDataProtocol, device, time);

    //    await SendMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    //public async Task<bool> SystemFastReadAsync(uint deviceUID, ushort mfxSID, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.FastRead, deviceUID, mfxSID);

    //    await SendMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    public async Task<CANMessage> SystemTrackProtocolAsync(uint deviceUID, byte param, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.TrackProtocol);
        message.SetData(param, 10);
        return await SendMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemNewRegistrationCounterAsync(uint deviceUID, ushort counter, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.FastRead, deviceUID, counter);

    //    await SendMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    public async Task<CANMessage> SystemOverloadAsync(uint deviceUID, byte channel, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.Overload);
        message.SetData(channel, 10);
        return await SendMessageAsync(message, cancellationToken);
    }

    public async Task<CANMessage> SystemResetAsync(uint deviceUID, byte target, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.Overload);
        message.SetData(target, 10);
        return await SendMessageAsync(message, cancellationToken);
    }

    #endregion

    #region Config Data

    private AutoResetEvent autoResetEventConfigDataStream = new AutoResetEvent(false);

    public async Task<string> ConfigDataLocosAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("loks");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataMagneticItemsAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("mags");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataRailwayRouteAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("fs");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataTrackDiagramAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("gbs");
        await SendMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    #endregion
}
