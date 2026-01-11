namespace CentralStationWebApi;

public class LocomotiveDirectionEventArgs(uint locomotiveId, DirectionChange direction) : LocomotiveEventArgs(locomotiveId)
{
    public DirectionChange Direction => direction;
}
