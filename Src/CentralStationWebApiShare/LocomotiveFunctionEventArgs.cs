using System;
using System.Collections.Generic;
using System.Text;

namespace CentralStationWebApi;

public class LocomotiveFunctionEventArgs(uint locomotiveId, byte function, byte value, ushort? functionValue = null) : EventArgs
{
    public uint LocomotiveId => locomotiveId;
    public byte Function => function;
    public byte Value => value;

    public bool HasFunctionValue => functionValue.HasValue;
    public ushort FunctionValue => functionValue.GetValueOrDefault();
}
