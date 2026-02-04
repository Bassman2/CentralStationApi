namespace CentralStationWebApi.Internal
{
    internal interface IProtocolHandler : IDisposable
    {
        void Connect(string host);

        void Send(CanMessage msg);

        //Task SendAsync(CanMessage msg);

        Task<CanMessage> ReceiveAsync();
    }
}
