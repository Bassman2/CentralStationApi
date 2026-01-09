namespace CentralStationWebApi.Internal;

internal class TrackCollector 
{
    private int counter = 0;
    private TrackData? data = null;
    private readonly Dictionary<uint, (int counter, TrackPageData? data)> dictionary = [];

    public void Clear()
    {
        counter = 0;
        data = null;
        dictionary.Clear();
    }

    public void Add(TrackData track)
    {
        data = track;
        foreach (var page in track.Pages ?? [])
        {
            dictionary[page.Id] = (0, null);
        }
    }

    public void Add(TrackPageData page)
    {
        dictionary[page.Page] = (0, page);
    }

    public bool ShouldRequest(out uint key)
    {
        if (data is null)
        {
            key = 0;
            return true;
        }
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

    //public void Init(IEnumerable<uint> keys)
    //{
    //    foreach (var key in keys)
    //    {
    //        dictionary[key] = (0, null);
    //    }
    //}



    //public T? this[uint key]
    //{
    //    get
    //    {
    //        return dictionary[key].data;
    //    }
    //    set
    //    {
    //        dictionary[key] = (0, value);
    //    }
    //}


    //public IEnumerable<T> Data => dictionary.Values.Where(static i => i is not null).Cast<T>(); 

    //public bool ShouldRequest(out uint key)
    //{

    //    var list = dictionary.Where(i => i.Value.data is null).ToList();
    //    if (list.Count > 0)
    //    {
    //        var res = list.FirstOrDefault();
    //        key = res.Key;
    //        return true;
    //    }
    //    key = 0;
    //    return false;
        
    //}

    public bool IsFinished => data is not null && dictionary.All(i => i.Value.data is not null);
}
