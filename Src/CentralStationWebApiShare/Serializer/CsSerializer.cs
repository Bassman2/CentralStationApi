namespace CentralStationWebApi.Serializer;

public static class CsSerializer
{
    public static T Deserialize<T>(Stream stream, string id) where T : ICsSerialize, new()
    {
        stream.Position = 0; 

        Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"Deserialize {typeof(T).Name} {id}");
        Debug.IndentLevel = 2;

        var main = new T();

        ICsSerialize[] stack = new ICsSerialize[8];

        StreamReader reader = new StreamReader(stream);
        ICsSerialize? leave = main;
        int curLevel = 0;

        stack[0] = main;

        int lineNum = 1;
        string? line = reader.ReadLine();
        if (line == null || !line.StartsWith(id))
        {
            throw new InvalidDataException("Invalid lokomotive file data");
        }
        Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"{lineNum++} {line}");

        while ((line = reader.ReadLine()) != null)
        {
            int lineLevel = GetLevel(line);
            
            Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"{lineNum++} {curLevel} {line}");

            if (lineLevel < curLevel)
            {
                Debug.Unindent();
                Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"End {leave.GetType().Name}");
                leave = stack[lineLevel];
                curLevel = lineLevel;
            }
            
            // subclasses
            if (line.IsSubitem())
            {
                stack[lineLevel] = leave;
                leave = leave.DeserializeLeave(line);
                Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"Begin {leave.GetType().Name}");
                Debug.Indent();
                curLevel++;
            }
            else
            {
                var parts = line.Split('=', 2);
                leave.DeserializeProperty(parts[0].TrimLevel(), parts[1]);
            }
        }
        Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, "Deserialize End");

        return main;
    }

    private static int GetLevel(this string line)
    {
        if (string.IsNullOrEmpty(line)) throw new InvalidDataException("Invalid line data");

        if (line[0] != ' ') return 0;
        int i = 1;
        while (i < line.Length && line[i] == '.') i++;
        return i - 1;
    } 
    
    private static string TrimLevel(this string line)
    {
        return line.TrimStart(' ', '.');
    }

    private static bool IsSubitem(this string line)
    {
        return !line.Contains('=');
    }

    public static ICsSerialize AddToList<T>(ref List<T>? list, T value) where T : ICsSerialize, new()
    {
        list ??= [];
        list.Add(value);
        return value;
    }
}
