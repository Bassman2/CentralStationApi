namespace CentralStationWebApi.Internal
{
    internal interface IProtocolHandler : IDisposable
    {
        void Connect(string host);

        void Send(CANMessage msg);

        //Task SendAsync(CANMessage msg);

        Task<CANMessage> ReceiveAsync();
    }
}
