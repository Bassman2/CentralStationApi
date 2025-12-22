using System.Globalization;

namespace CentralStationWebApi.Model;

public class CsLocomotive : ICsSerialize
{
    public void Deserialize(StreamReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            // subclasses
            if (!line.Contains('='))
            {
                
                throw new InvalidDataException($"Unknown section {line}");
            }
            else
            {
                var parts = line.Split('=', 2);
                switch (parts[0])
                {
                case " .uid":
                    UID = uint.Parse(parts[1], NumberStyles.HexNumber );
                    break;
                case " .name":
                    Name = parts[1];
                    break;
                default:
                    throw new InvalidDataException($"Unknown lokomotive section {line}");
                }
            }
        }

    }

    public uint UID { get; private set; }

    public string? Name { get; private set; }
}
