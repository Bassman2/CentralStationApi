namespace CentralStationDemo.Model;

public class FunctionModel
{
    public FunctionModel()
    { }

    public FunctionModel(Function function)
    {
        Num = function.Num;
        Type = function.Type;
        Value = function.Value;
    }

    [JsonPropertyName("nr")]
    public int Num { get; set; }

    [JsonPropertyName("typ")]
    public int Type { get; set; }

    [JsonPropertyName("value")]
    public int Value { get; set; }
}
