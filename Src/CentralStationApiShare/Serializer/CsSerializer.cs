namespace CentralStationApi.Serializer;

/// <summary>
/// Provides methods for deserializing Central Station configuration files (CS2/CS3 format).
/// Handles the custom text-based configuration format used by Märklin Central Station.
/// </summary>
/// <remarks>
/// The CS2/CS3 format is a hierarchical, indented text format where:
/// <list type="bullet">
/// <item><description>Indentation levels are indicated by spaces followed by dots (e.g., " .", " ..")</description></item>
/// <item><description>Properties are defined as name=value pairs</description></item>
/// <item><description>Nested sections are indicated by names without '=' characters</description></item>
/// <item><description>Numeric values can be decimal or hexadecimal (prefixed with "0x")</description></item>
/// </list>
/// </remarks>
public static class CsSerializer
{
    /// <summary>
    /// Deserializes a Central Station configuration file from a stream into a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into. Must implement <see cref="ICsSerialize"/> and have a parameterless constructor.</typeparam>
    /// <param name="stream">The stream containing the CS2/CS3 format configuration data.</param>
    /// <returns>A deserialized object of type <typeparamref name="T"/> populated with data from the stream.</returns>
    /// <exception cref="InvalidDataException">Thrown when the stream contains invalid or malformed data.</exception>
    /// <remarks>
    /// The method processes the hierarchical structure by tracking indentation levels and
    /// delegating property and section parsing to the <see cref="ICsSerialize"/> implementation.
    /// Diagnostic output can be controlled via <see cref="TraceSwitches.SerializerSwitch"/>.
    /// </remarks>
    public static T Deserialize<T>(Stream stream) where T : ICsSerialize, new()
    {
        stream.Position = 0;

        Debug.IndentLevel = 0;
        Debug.IndentSize = 2;
        Debug.WriteLineIf(TraceSwitches.SerializerSwitch.TraceInfo, $"Deserialize {typeof(T).Name}");

        var root = new T();
        ICsSerialize? leave = root;

        ICsSerialize[] stack = new ICsSerialize[8];
        stack[0] = root;

        StreamReader reader = new StreamReader(stream);
        
        int curLevel = 0;        
        int lineNum = 1;
        string? line = reader.ReadLine();
        //if (line == null)
        //{
        //    throw new InvalidDataException("Invalid lokomotive file data");
        //}
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
    
    /// <summary>
    /// Converts a string value to a 32-bit signed integer.
    /// Supports both decimal and hexadecimal (0x-prefixed) formats.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The parsed integer value.</returns>
    public static int ToInt(string value)
    {
        return value.StartsWith("0x") ? Convert.ToInt32(value, 16) : int.Parse(value);
    }

    /// <summary>
    /// Converts a string value to a 32-bit unsigned integer.
    /// Supports both decimal and hexadecimal (0x-prefixed) formats.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The parsed unsigned integer value.</returns>
    public static uint ToUInt(string value)
    {
        return value.StartsWith("0x") ? Convert.ToUInt32(value, 16) : uint.Parse(value);
    }

    /// <summary>
    /// Converts a string value to a 16-bit signed integer.
    /// Supports both decimal and hexadecimal (0x-prefixed) formats.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The parsed short integer value.</returns>
    public static short ToShort(string value)
    {
        return value.StartsWith("0x") ? Convert.ToInt16(value, 16) : short.Parse(value);
    }

    /// <summary>
    /// Converts a string value to a 16-bit unsigned integer.
    /// Supports both decimal and hexadecimal (0x-prefixed) formats.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The parsed unsigned short integer value.</returns>
    public static ushort ToUShort(string value)
    {
        return value.StartsWith("0x") ? Convert.ToUInt16(value, 16) : ushort.Parse(value);
    }

    /// <summary>
    /// Converts a space-separated string of values to an array of 32-bit unsigned integers.
    /// Each value can be in decimal or hexadecimal (0x-prefixed) format.
    /// </summary>
    /// <param name="value">The space-separated string of values.</param>
    /// <returns>An array of parsed unsigned integer values.</returns>
    public static uint[] ToUIntArray(string value)
    {
        return [.. value.Split(' ', StringSplitOptions.TrimEntries).Select(i => ToUInt(i))]; 
    }

    /// <summary>
    /// Converts a space-separated string of values to an array of 32-bit signed integers.
    /// Each value can be in decimal or hexadecimal (0x-prefixed) format.
    /// </summary>
    /// <param name="value">The space-separated string of values.</param>
    /// <returns>An array of parsed integer values.</returns>
    public static int[] ToIntArray(string value)
    {
        return [.. value.Split(' ', StringSplitOptions.TrimEntries).Select(i => ToInt(i))];
    }

    /// <summary>
    /// Converts a string value to a boolean.
    /// Any non-zero integer value is considered true; zero is false.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>True if the value is non-zero; otherwise, false.</returns>
    public static bool ToBool(string value)
    {
        return ToInt(value) != 0;
    }
}
