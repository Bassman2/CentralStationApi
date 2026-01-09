namespace CentralStationWebApi.Internal;

internal class ControllerCollector
{
    //private int counter = 0;
    private readonly Dictionary<uint, (int counter, Controller controller)> dictionary = [];

    public void Clear()
    {
        //counter = 0;
        dictionary.Clear();
    }

    public void Add(Controller controller)
    {
        dictionary[controller.DeviceId] = (0, controller);
    }
    
    public void Add(uint deviceId, int index, DataCollector dataCollector)
    {
        dictionary[deviceId].controller.Add(index, dataCollector);
    }

    public bool ShouldRequest(out (uint deviceId, uint index) req)
    {
        req = (0, 0);

        var list = dictionary.Where(i => !i.Value.controller.HasStatusData).ToList();

        if (list.Count > 0)
        {
            var ctrl = list.FirstOrDefault();
            req = (ctrl.Key, 0);
            return true;
        }
        return false;
    }

    // public void 


}
