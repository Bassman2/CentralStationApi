namespace CentralStationWebApi;

partial class CentralStationBasic
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

            client.Send(reqMsg);
            //// send message
            //if (protocol == Protocol.TCP)
            //{

            //}
            //else
            //{
            //    senderClient!.Send(reqMsg.Buffer, 13);
            //}
        
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

            client.Send(reqMsg);

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
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Stop);
        var res = await SendSingleMessageAsync(message, cancellationToken);
        return res.Success;
    }

    public async Task<MessageResponse> SystemGoAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Go);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Halt);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemLocoHaltAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoHalt);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemLocoCycleStopAsync(uint device = AllDevices, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoCycleStop);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemLocoDataProtocolAsync(uint device = AllDevices, byte protocoll = 0xff, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.LocoDataProtocol).
            AddByte(protocoll);         
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemSwitchingTimelAsync(uint device, ushort time, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.SwitchingTime).
            AddUInt16(time);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemFastReadAsync(uint device, ushort mfxSid, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.FastRead).
            AddUInt16(mfxSid);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemTrackProtocolAsync(uint deviceUID, byte protocoll, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(deviceUID).
            AddSubCommand(SubCommand.TrackProtocol).
            AddByte(protocoll);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemNewRegistrationCounterAsync(uint device, ushort newRegistrationCounter, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.NewRegistrationCounter).
            AddUInt16(newRegistrationCounter);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemOverloadAsync(uint deviceUID, byte channel, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(deviceUID).
            AddSubCommand(SubCommand.Overload).
            AddByte(channel);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemStatusAsync(uint device, byte channel, ushort? value = null, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Status).
            AddByte(channel).
            AddUInt16(value);   // optional
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemIdentifierAsync(uint device, byte identifier, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.Identifier).
            AddUInt16(identifier);   // optional
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemMfxSeekAsync(uint device, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(device).
            AddSubCommand(SubCommand.MfxSeek);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    public async Task<MessageResponse> SystemResetAsync(uint deviceUID, byte target, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.SystemCommand, hash).
            AddUInt32(deviceUID).
            AddSubCommand(SubCommand.Overload).
            AddByte(target);
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion

    #region 3 Administration

    public async Task<bool> LocoDirectionAsync(uint device, ushort time, CancellationToken cancellationToken = default)
    {
        var msg = new CANMessage(Priority.Proirity1, Command.LocoDirection, hash);

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

    private AutoResetEvent autoResetEventConfigDataStream = new(false);

    public async Task<string> ConfigDataLocosAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("loks");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataMagneticItemsAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("mags");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataRailwayRouteAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("fs");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<string> ConfigDataTrackDiagramAsync(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("gbs");
        await SendSingleMessageAsync(message, cancellationToken);

        autoResetEventConfigDataStream.WaitOne();
        return "";
    }

    public async Task<MessageResponse> ConfigDataLocomotivesInfo(CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.RequestConfigData, hash).
            AddString("lokinfo");
        return await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion

    #region 9 Automation

    public async Task AutomaticTransmissionAsync(ushort deviceExpert, ushort automaticFunction, byte position, byte parameter, CancellationToken cancellationToken = default)
    {
        var message = new CANMessage(Priority.Proirity1, Command.AutomaticTransmission, hash).
            AddUInt16(deviceExpert).
            AddUInt16(automaticFunction).
            AddByte(position).
            AddByte(parameter);
        await SendSingleMessageAsync(message, cancellationToken);
    }

    #endregion
}
