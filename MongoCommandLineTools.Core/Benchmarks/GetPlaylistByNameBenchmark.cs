using System.Diagnostics;
using MongoCommandLineTools.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCommandLineTools.Core.Benchmarks;

public class GetPlaylistByNameBenchmark(string playlistName) : IBenchmark
{
    public async Task RunBenchmarkAsync(string connectionString, string databaseName, string collectionName, int iterations)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);

        var filter = Builders<BsonDocument>.Filter.Eq("name", playlistName);
        var stopwatch = new Stopwatch();
        
        var lowestElapsedMilliseconds = long.MaxValue;
        var highestElapsedMilliseconds = long.MinValue;
        var totalElapsedMilliseconds = 0L;

        for (var i = 0; i < iterations; i++)
        {
            stopwatch.Restart();
            await collection.Find(filter).FirstOrDefaultAsync();
            stopwatch.Stop();
            
            totalElapsedMilliseconds += stopwatch.ElapsedMilliseconds;
            
            if (stopwatch.ElapsedMilliseconds < lowestElapsedMilliseconds)
            {
                lowestElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }
            
            if (stopwatch.ElapsedMilliseconds > highestElapsedMilliseconds)
            {
                highestElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }
        }
        
        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine("Benchmark name: GetPlaylistByName");
        Console.WriteLine("Description: Get playlist by name");
        Console.WriteLine($"Iterations: {iterations}\n");
        Console.WriteLine($"Total elapsed time: {totalElapsedMilliseconds} ms");
        Console.WriteLine($"Average time to get track by name '{playlistName}': {totalElapsedMilliseconds / iterations} ms");
        Console.WriteLine($"Lowest elapsed time: {lowestElapsedMilliseconds} ms");
        Console.WriteLine($"Highest elapsed time: {highestElapsedMilliseconds} ms");
        Console.WriteLine("-----------------------------------------------");
    }
}