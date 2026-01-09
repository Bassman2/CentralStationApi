using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CentralStationWebApi.Internal;

internal class DictionaryCollector<T> where T : class 
{
    private int counter = 0;
    private readonly Dictionary<uint, T?> dictionary = [];

    public void Clear()
    {
        counter = 0;
        dictionary.Clear();
    }

    public void Increment() => counter++;

    public IEnumerable<T> Data => dictionary.Values.Where(static i => i is not null).Cast<T>(); 
    
    public bool ShouldRequest(out uint key)
    {
        
        counter++;
        var res = dictionary.Where(i => i.Value is not null).FirstOrDefault();
        if (res.Value is not null)
        {
            key = res.Key;
            return true;
        }
        key = 0;
        return false;
        
    }

    public bool IsFinished => dictionary.Count == 0;
}
