using System.CommandLine;
using CentralStationApi;

namespace CentralStationTracer;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        RootCommand rootCommand = new("Central Station CAN message tracer");

        Option<Protocol> protocolOption = new("--protocol", "-p")
        {
            Description = "Protocol of the connection",
            DefaultValueFactory = _ => Protocol.TCP
        };
        rootCommand.Options.Add(protocolOption);

        Option<string> nameOption = new("--name", "-n")
        {
            Description = "Host name of the Central Station Internet connection or COM port of the USB to CAN adapter e.q. COM4",
            DefaultValueFactory = _ => "CS3"
        };
        rootCommand.Options.Add(nameOption);        

        Option<string> outputOption = new("--output", "-o")
        {
            Description = "Trace output file"
        };
        rootCommand.Options.Add(outputOption);

        rootCommand.SetAction(parseResult =>
        {
             return new Tracer().Trace(
                 parseResult.GetValue(protocolOption),
                 parseResult.GetValue(nameOption),
                 parseResult.GetValue(outputOption));
        });

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
