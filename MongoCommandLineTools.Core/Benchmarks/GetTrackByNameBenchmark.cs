using System.Diagnostics;
using MongoCommandLineTools.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCommandLineTools.Core.Benchmarks;

public class GetTrackByNameBenchmark(string trackName) : IBenchmark
{
    public async Task RunBenchmarkAsync(string connectionString, string databaseName, string collectionName, int iterations)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);

        var filter = Builders<BsonDocument>.Filter.Eq("tracks.track_name", trackName);
        var stopwatch = new Stopwatch();

        for (var i = 0; i < iterations; i++)
        {
            stopwatch.Start();
            await collection.Find(filter).FirstOrDefaultAsync();
            stopwatch.Stop();
        }

        Console.Clear();
        Console.WriteLine($"Average time to get track by name '{trackName}': {stopwatch.ElapsedMilliseconds / iterations} ms");
    }
}