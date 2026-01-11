namespace CentralStationWebApi;

public class LocomotiveEventArgs(uint locomotiveId) : EventArgs
{
    public uint LocomotiveId => locomotiveId;
}
