namespace CentralStationWebApi.Internal;

internal static class Tracer
{
    private const string folder = @"C:\\MärklinTraces";

    private static bool trace = Directory.Exists(folder);

    public static void TraceStream(Stream stream, string fileName)
    {
        if (trace)
        {
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string? firstLine = reader.ReadLine();

            string fileId = firstLine?.Trim('[', ']') ?? "unknown";

            using var file = File.CreateText(Path.Combine(folder, $"{fileName}-{fileId}.cs2"));
            file.WriteLine(firstLine);
            file.Write(reader.ReadToEnd());

            stream.Position = 0;
        }
    }

    private static StreamWriter? writer = null;
    public static void TraceMessage(CANMessage msg)
    {
        if (trace)
        {
            writer ??= new StreamWriter(Path.Combine(folder, $"messages.txt"));
            writer.WriteLine(msg.ToTrace());
            writer.Flush();
        }
    }
}
