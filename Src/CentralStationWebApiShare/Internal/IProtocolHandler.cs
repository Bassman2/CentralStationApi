namespace CentralStationWebApi.Internal
{
    internal interface IProtocolHandler : IDisposable
    {
        void Connect(string host);

        bool IsConnected { get; }

        void Send(CanMessage msg);

        Task SendAsync(CanMessage msg);

        Task<CanMessage> ReceiveAsync();
    }
}
