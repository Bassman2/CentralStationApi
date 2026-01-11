namespace CentralStationWebApi;

public class LocomotiveFunctionEventArgs(uint locomotiveId, byte function, byte value, ushort? functionValue = null) : LocomotiveEventArgs(locomotiveId)
{
    public byte Function => function;
    public byte Value => value;
        
    public bool HasFunctionValue => functionValue.HasValue;
    public ushort FunctionValue => functionValue.GetValueOrDefault();
    
}
