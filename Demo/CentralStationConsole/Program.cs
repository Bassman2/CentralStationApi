using CentralStationWebApi;

namespace CentralStationConsole
{
    internal class Program
    {
        static async Task Main(/*string[] args*/)
        {

            using var cs = new CentralStation("CS2");

            
            //await cs.SystemStopAsync();

            Console.WriteLine("Ready");

            await Task.Delay(TimeSpan.FromSeconds(30));

            Console.WriteLine("End");
        }
    }
}
