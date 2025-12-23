using System;
using System.Globalization;

namespace CentralStationWebApi.Model;

public class CsLocomotive : ICsSerialize
{
    public ICsSerialize DeserializeLeave(string line)
    {
        switch (line.Trim(' ', '.'))
        {
        case "funktionen":
            var function = new CsFunction();
            Functions ??= [];
            Functions.Add(function);
            return function;
        case "funktionen_2":
            var function2 = new CsFunction();
            Functions2 ??= [];
            Functions2.Add(function2);
            return function2;
        default:
            throw new InvalidDataException($"Unknown locomotive section {line}");
        }
    }

    public void DeserializeProperty(string name, string value)
    {
        switch (name)
        {
        case "name":
            Name = value; 
            break;
        case "uid":
            Uid = Convert.ToUInt32(value, 16); 
            break;
        case "mfxuid":
            MfxUid = Convert.ToUInt32(value, 16);
            break;
        case "adresse":
            Adresse = Convert.ToUInt32(value, 16);
            break;
        case "icon":
            Icon = value;
            break;
        case "typ":
            Type = value;
            break;



        case "mfxtyp":
            MfxTyp = byte.Parse(value);
            break;
        case "blocks":
            Blocks = value.Split(' ', StringSplitOptions.TrimEntries).Select(i => uint.Parse(i)).ToArray();
            break;
        default:
            //throw new InvalidDataException($"Unknown locomotive property {name}");
            break;
        }
    }

    public List<CsFunction>? Functions { get; private set; }
    public List<CsFunction>? Functions2 { get; private set; }
    public string? Name { get; private set; }
    public uint Uid { get; private set; }
    public uint MfxUid { get; private set; }
    public uint Adresse { get; private set; }
    public string? Icon { get; private set; }
    public string? Type { get; private set; }

    //
    public byte MfxTyp { get; private set; }
    public uint[]? Blocks { get; private set; }
}
