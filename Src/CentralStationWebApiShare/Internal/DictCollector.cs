namespace CentralStationWebApi.Internal;

internal class DictCollector<T> where T : class 
{
    private readonly Dictionary<uint, (int counter, T? data)> dictionary = [];

    public void Clear()
    {
        dictionary.Clear();
    }

    public void Init(IEnumerable<uint> keys)
    {
        foreach (var key in keys)
        {
            dictionary[key] = (0, null);
        }
    }

    

    public T? this[uint key]
    {
        get
        {
            return dictionary[key].data;
        }
        set
        {
            dictionary[key] = (0, value);
        }
    }


    //public IEnumerable<T> Data => dictionary.Values.Where(static i => i is not null).Cast<T>(); 
    
    public bool ShouldRequest(out uint key)
    {

        var list = dictionary.Where(i => i.Value.data is null).ToList();
        if (list.Count > 0)
        {
            var res = list.FirstOrDefault();
            key = res.Key;
            return true;
        }
        key = 0;
        return false;
        
    }

    public bool IsFinished => dictionary.All(i => i.Value.data is not null);
}
