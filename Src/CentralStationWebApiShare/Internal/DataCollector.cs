using System.Collections.Generic;

namespace CentralStationWebApi.Internal;

internal class DataCollector<T> where T : class
{
    private int counter = 0;
    private T? data = null;

    public void Clear()
    { 
        counter = 0;
        data = null;
    }

    public void Increment() => counter++;

    public T? Data
    {
        get => data;
        set => data = value;
    }

    public bool ShouldRequest
    {
        get
        {
            counter++;
            return data == null;
        }
    }

    public bool IsFinished => data != null;
}


