using System.Threading;

namespace CentralStationWebApi;

partial class CentralStation
{
    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(30);

    #region Private Message Response

    private readonly List<CANMessage> messageQueue = [];

    private readonly AutoResetEvent messageReceivedEvent = new(false);


    private void HandleAsync(CANMessage msg)
    {
        var resMsg = messageQueue.FirstOrDefault(m => m.IsResponseMsgFrom(msg));
        if (resMsg is not null)
        {
            messageReceivedEvent.Set();
        }
    }

    private async Task<MessageResponse> SendSingleMessageAsync(CANMessage reqMsg, CancellationToken cancellationToken = default)
    {
        // create wait thread
        return await Task.Run(() =>
        {
            // add to message queue
            messageReceivedQueue.Add(reqMsg);

            // send message
            sender.Send(reqMsg.Buffer, 13);

        
            while (messageReceivedEvent.WaitOne(ReceiveTimeout))
            {
                var resMsg = messageQueue.FirstOrDefault(m => m.IsResponseMsgFrom(reqMsg));
                if (resMsg is not null)
                {
                    messageQueue.Remove(reqMsg);
                    return new MessageResponse(reqMsg, resMsg); // response arrived
                }
            }
            return new MessageResponse(reqMsg);  // failure
        });
    }

    private async Task<MessageResponse> SendStreamMessageAsync(CANMessage reqMsg, CancellationToken cancellationToken = default)
    {
        // create wait thread
        return await Task.Run(() =>
        {
            // add to message queue
            messageReceivedQueue.Add(reqMsg);

            // send message
            sender.Send(reqMsg.Buffer, 13);


            while (messageReceivedEvent.WaitOne(ReceiveTimeout))
            {
                var resMsg = messageQueue.FirstOrDefault(m => m.IsResponseMsgFrom(reqMsg));
                if (resMsg is not null)
                {
                    messageQueue.Remove(reqMsg);
                    return new MessageResponse(reqMsg, resMsg); // response arrived
                }
            }
            return new MessageResponse(reqMsg);  // failure
        });
    }

    #endregion

    #region 2 System Commands

    public async Task<bool> SystemStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash, 5);
        message.SetData(SystemCommand.Stop);
        var res = await SendSingleMessageAsync(message, cancellationToken);
        return res.Success;
    }

    public async Task<MessageResponse> SystemGoAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash, 5);
        message.SetData(device, 5);
        message.SetData(SystemCommand.Go);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash, 5);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.Halt);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemLocoHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoHalt);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemLocoCycleStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoCycleStop);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemLocoDataProtocolAsync(uint device = AllDevices, byte protocoll = 0xff, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 5;
        message.SetData(device, 5);
        message.SetData(SystemCommand.LocoDataProtocol);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemFastReadAsync(uint deviceUID, ushort mfxSID, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.FastRead, deviceUID, mfxSID);

    //    await SendSingleMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    public async Task<MessageResponse> SystemTrackProtocolAsync(uint deviceUID, byte param, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash);
        message.DataLength = 6;
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.TrackProtocol);
        message.SetData(param, 10);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    //public async Task<bool> SystemNewRegistrationCounterAsync(uint deviceUID, ushort counter, CancellationToken cancellationToken = default)
    //{
    //    var msg = new SystemMessage(SystemCommand.FastRead, deviceUID, counter);

    //    await SendSingleMessageAsync(msg, cancellationToken);

    //    return true;
    //}

    public async Task<MessageResponse> SystemOverloadAsync(uint deviceUID, byte channel, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash, 6);
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.Overload);
        message.SetData(channel, 10);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemResetAsync(uint deviceUID, byte target, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash, 6);
        message.SetData(deviceUID, 5);
        message.SetData(SystemCommand.Overload);
        message.SetData(target, 10);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion

    #region 3 Administration

    public async Task<bool> LocoDirectionAsync(uint device, ushort time, CancellationToken cancellationToken = default)
    {
        var msg = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash, 0);

        await SendSingleMessageAsync(msg, cancellationToken);

        return true;
    }

    #endregion

    #region 4 Article Commands

    #endregion

    #region 5 Feedback

    #endregion

    #region 6 Other Commands / Sonstige Befehle

    public async Task ParticipantsAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SoftwareVersion, hash);
        await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion

    #region 7 GUI Information Transfer

    private AutoResetEvent autoResetEventConfigDataStream = new AutoResetEvent(false);

    public async Task<string> ConfigDataLocosAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("loks");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataMagneticItemsAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("mags");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataRailwayRouteAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("fs");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataTrackDiagramAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("gbs");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<MessageResponse> ConfigDataLocomotivesInfo(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash);
        message.DataLength = 8;
        message.SetData("lokinfo");
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion

    #region 9 Automation

    public async Task AutomaticTransmissionAsync(ushort deviceExpert, ushort automaticFunction, byte position, byte parameter, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.AutomaticTransmission, hash, 6);
        message.SetData((ushort)deviceExpert, 5);
        message.SetData((ushort)automaticFunction, 7);
        message.SetData((byte)position, 9);
        message.SetData((byte)parameter, 10);
        await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion
}
