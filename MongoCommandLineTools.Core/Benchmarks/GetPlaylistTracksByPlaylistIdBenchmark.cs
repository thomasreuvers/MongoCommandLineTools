using System.Diagnostics;
using MongoCommandLineTools.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCommandLineTools.Core.Benchmarks;

public class GetPlaylistTracksByPlaylistIdBenchmark(string playlistId) : IBenchmark
{
    public async Task RunBenchmarkAsync(string connectionString, string databaseName, string collectionName, int iterations)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", playlistId);
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
        Console.WriteLine("Benchmark name: GetPlaylistTracksByPlaylistId");
        Console.WriteLine("Description: Get tracks by playlist ID");
        Console.WriteLine($"Iterations: {iterations}\n");
        Console.WriteLine($"Total elapsed time: {totalElapsedMilliseconds} ms");
        Console.WriteLine($"Average time to get tracks by playlist ID '{playlistId}': {totalElapsedMilliseconds / iterations} ms");
        Console.WriteLine($"Lowest elapsed time: {lowestElapsedMilliseconds} ms");
        Console.WriteLine($"Highest elapsed time: {highestElapsedMilliseconds} ms");
        Console.WriteLine("-----------------------------------------------");
    }
}