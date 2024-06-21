using System.Diagnostics;
using MongoCommandLineTools.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCommandLineTools.Core.Benchmarks;

public class UpdateDocumentBenchmark : IBenchmark
{
    public async Task RunBenchmarkAsync(string connectionString, string databaseName, string collectionName, int iterations)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);

        var stopwatch = new Stopwatch();
        var lowestElapsedMilliseconds = long.MaxValue;
        var highestElapsedMilliseconds = long.MinValue;
        var totalElapsedMilliseconds = 0L;

        for (var i = 0; i < iterations; i++)
        {
            // Ensure pid is correctly initialized to prevent potential null reference issues during filter creation
            var pid = 1000001 + i;
            var filter = Builders<BsonDocument>.Filter.Eq("pid", pid);
            var update = Builders<BsonDocument>.Update.Inc("num_followers", 1);

            stopwatch.Restart();
            await collection.UpdateOneAsync(filter, update);
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
        Console.WriteLine("Benchmark name: UpdateDocument");
        Console.WriteLine("Description: Update document");
        Console.WriteLine($"Iterations: {iterations}\n");
        Console.WriteLine($"Total elapsed time: {totalElapsedMilliseconds} ms");
        Console.WriteLine($"Average time to update document: {totalElapsedMilliseconds / iterations} ms");
        Console.WriteLine($"Lowest elapsed time: {lowestElapsedMilliseconds} ms");
        Console.WriteLine($"Highest elapsed time: {highestElapsedMilliseconds} ms");
        Console.WriteLine("-----------------------------------------------");
    }
}
