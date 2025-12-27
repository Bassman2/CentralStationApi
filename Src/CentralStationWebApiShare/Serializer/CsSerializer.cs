namespace CentralStationWebApi.Serializer;

public static class CsSerializer
{
    public static T Deserialize<T>(Stream stream, string id) where T : ICsSerialize, new()
    {
        stream.Position = 0;

        Debug.IndentLevel = 0;
        Debug.IndentSize = 2;
        Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"Deserialize {typeof(T).Name} {id}");

        var root = new T();
        ICsSerialize? leave = root;

        ICsSerialize[] stack = new ICsSerialize[8];
        stack[0] = root;

        StreamReader reader = new StreamReader(stream);
        
        int curLevel = 0;        
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
            
            Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"{lineNum++} ({lineLevel}/{curLevel}) '{line}' ---- [{TraceStack(stack)}]\"");

            if (lineLevel < curLevel)
            {
                Debug.Unindent();
                Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"End ({lineLevel}/{curLevel}) {leave.GetType().Name} ---- [{TraceStack(stack)}]\"");
                leave = stack[lineLevel];
                //curLevel = lineLevel;
            }
            
            // subclasses
            if (line.IsSubitem())
            {
                stack[lineLevel] = leave;
                leave = leave.DeserializeLeave(line.TrimLevel());
                Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"Begin ({lineLevel}/{curLevel}) {leave.GetType().Name} ---- [{TraceStack(stack)}]");
                Debug.Indent();
                //curLevel++;
            }
            else
            {
                var parts = line.Split('=', 2);
                leave.DeserializeProperty(parts[0].TrimLevel(), parts[1]);
            }
            curLevel = lineLevel;
        }
        Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, "Deserialize End");

        return root;
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

    //public static ICsSerialize AddToList<T>(ref List<T>? list, T value) where T : ICsSerialize, new()
    //{
    //    list ??= [];
    //    list.Add(value);
    //    return value;
    //}

    private static string TraceStack(ICsSerialize[] stack) => string.Join(", ", stack.Select(i => i?.GetType().Name ?? "null"));
    
    public static uint ToUInt(string value)
    {
        return value.StartsWith("0x") ? Convert.ToUInt32(value, 16) : uint.Parse(value);
    }

    public static int ToInt(string value)
    {
        return value.StartsWith("0x") ? Convert.ToInt32(value, 16) : int.Parse(value);
    }

    public static uint[] ToUIntArray(string value)
    {
        return value.Split(' ', StringSplitOptions.TrimEntries).Select(i => ToUInt(i)).ToArray(); 
    }

    public static int[] ToIntArray(string value)
    {
        return value.Split(' ', StringSplitOptions.TrimEntries).Select(i => ToInt(i)).ToArray();
    }

    public static bool ToBool(string value)
    {
        return ToInt(value) == 0 ? false : true;
    }
}
