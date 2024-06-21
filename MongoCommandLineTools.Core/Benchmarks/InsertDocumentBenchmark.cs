using System.Diagnostics;
using MongoCommandLineTools.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCommandLineTools.Core.Benchmarks;

public class InsertDocumentBenchmark : IBenchmark
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
            var name = $"test{i}";
            
            var document = new BsonDocument
            {
                { "name", name },
                { "collaborative", false },
                { "pid", 1000001 + i },
                { "modified_at", 1435708800 },
                { "num_tracks", 144 },
                { "num_albums", 117 },
                { "num_followers", 1 },
                { "tracks", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "pos", 0 },
                            { "artist_name", "Sander van Doorn" },
                            { "track_uri", "spotify:track:4zXQSm5XIkcn5qtmtbwO9O" },
                            { "artist_uri", "spotify:artist:22bukBZvUppuwQwmDz75Gz" },
                            { "track_name", "Gold Skies - Original Mix" },
                            { "album_uri", "spotify:album:01wwlmULNFwk6YZ8riFRfL" },
                            { "duration_ms", 329067 },
                            { "album_name", "Gold Skies (Remixes)" }
                        },
                        new BsonDocument
                        {
                            { "pos", 1 },
                            { "artist_name", "Martin Garrix" },
                            { "track_uri", "spotify:track:6JEntXLt4z98CcDtIH9sU7" },
                            { "artist_uri", "spotify:artist:60d24wfXkVzDSfLS6hyCjZ" },
                            { "track_name", "Animals - Extended" },
                            { "album_uri", "spotify:album:55HXtmTsfepv0mY9vIIFhi" },
                            { "duration_ms", 303826 },
                            { "album_name", "Animals" }
                        },
                        new BsonDocument
                        {
                            { "pos", 2 },
                            { "artist_name", "Martin Garrix" },
                            { "track_uri", "spotify:track:54fgYLd9UtdPZVjx548Ver" },
                            { "artist_uri", "spotify:artist:60d24wfXkVzDSfLS6hyCjZ" },
                            { "track_name", "Proxy" },
                            { "album_uri", "spotify:album:4EV2HGPHQpUu4cPxJTP0OT" },
                            { "duration_ms", 277546 },
                            { "album_name", "Gold Skies" }
                        }
                    }
                }
            };

            stopwatch.Restart();
            await collection.InsertOneAsync(document);
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
        Console.WriteLine("Benchmark name: InsertDocument");
        Console.WriteLine("Description: Insert a document");
        Console.WriteLine($"Iterations: {iterations}\n");
        Console.WriteLine($"Total elapsed time: {totalElapsedMilliseconds} ms");
        Console.WriteLine($"Average time to insert document: {totalElapsedMilliseconds / iterations} ms");
        Console.WriteLine($"Lowest elapsed time: {lowestElapsedMilliseconds} ms");
        Console.WriteLine($"Highest elapsed time: {highestElapsedMilliseconds} ms");
        Console.WriteLine("-----------------------------------------------");
    }
}