using CentralStationWebApi;

namespace CentralStationConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            using var cs = new CentralStation();

            
            await cs.SystemStopAsync();

            Console.WriteLine("End");
        }
    }
}
