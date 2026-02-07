using System;
using System.Diagnostics;
using System.Reflection;

namespace CentralStationImageCache;

internal static class Program
{
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var client = new HttpClient() { Timeout = new TimeSpan(0, 0, 2) };

        await client.LoadArticlesAsync(new Uri("http://cs3/app/assets/mag/"), "D:\\_Data\\Märklin\\magicon", 0, 500, 100);
        await client.LoadFunctionsAsync(new Uri("http://cs3/app/assets/fct/"), "D:\\_Data\\Märklin\\function", 0 , 999);


    }

   // private const string articlesDir = "D:\\_Data\\Märklin\\magicon";

    private static async Task LoadArticlesAsync(this HttpClient client, Uri location, string store, int from, int to, int num)
    {
        Directory.CreateDirectory(store);

        for (int index = from; index <= to; index++)
        {
            for (int val = 0; val < num; val++)
            {

                await client.LoadFileAsync(location, store, $"magicon_i_{index:000}_{val:00}.svg");
                await client.LoadFileAsync(location, store, $"magicon_a_{index:000}_{val:00}.svg");
            }
        }
    }

    //private const string functionsDir = "D:\\_Data\\Märklin\\function";

    private static async Task LoadFunctionsAsync(this HttpClient client, Uri location, string store, int from, int to)
    {
        Directory.CreateDirectory(store);

        for (int index = from; index <= to; index++)
        {
            await client.LoadFileAsync(location, store, $"fkticon_i_{index:000}.svg");
            await client.LoadFileAsync(location, store, $"fkticon_a_{index:000}.svg");
        }
    }

    private static async Task LoadFileAsync(this HttpClient client, Uri location, string filePath, string fileName)
    {
        Console.WriteLine(fileName);

        Uri uri = new Uri(location, fileName);
        try
        {
            using var stream = await client.GetStreamAsync(uri);
            using var writer = File.Create(Path.Combine(filePath, fileName));
            await stream.CopyToAsync(writer);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading {uri}: {ex}");
        }
    }
}
