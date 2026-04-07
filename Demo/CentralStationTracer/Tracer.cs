using CentralStationApi;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace CentralStationTracer;

internal class Tracer
{
    public int Trace(Protocol protocol, string? name, string? outputFile)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        try
        {
            StreamWriter? writer; // = string.IsNullOrWhiteSpace(outputFile) ? null : File.CreateText(outputFile);

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
                Console.SetOut(writer);
            }
            else
            {
                writer = File.CreateText(outputFile);
            }

            writer.WriteLine("Start");

            CentralStation centralStation = new();
            centralStation.MessageReceived += (sender, message) =>
            {
                writer.WriteLine(message.Message.ToTrace());
            };

            centralStation.Connect(name, protocol);

            while (centralStation.IsConnected)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    return 0;
                }
                Thread.Sleep(500);
            }
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        
    }
                 
}
