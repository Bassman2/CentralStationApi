using System;

namespace CentralStationImageCache
{
    internal class Program
    {
        private const string dir = "D:\\_Data\\Märklin\\magicon";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            Directory.CreateDirectory(dir);

            var client = new HttpClient() { Timeout = new TimeSpan(0, 0, 2) };

            for (int index = 200; index <= 500; index++)
            {
                for (int val = 0; val < 100; val++)
                {
                    string fileName = $"magicon_a_{index:000}_{val:00}.svg";
                    string filePath = Path.Combine(dir, fileName);

                    Console.WriteLine(fileName);

                                               
                    Uri uri = new Uri(new Uri("http://cs3/app/assets/mag/"), fileName);
                    try
                    {
                        using var stream = await client.GetStreamAsync(uri);
                        using var writer = File.Create(filePath);
                        await stream.CopyToAsync(writer);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}
