using MongoCommandLineTools.Core.Benchmarks;
using MongoCommandLineTools.Core.Interfaces;

namespace MongoCommandLineTools.Core.Services;

public class BenchmarkProcessorService : IBenchmarkProcessorService
{
    public async Task RunBenchmarkAsync(
        string benchmarkName,
        string connectionString,
        string databaseName,
        string collectionName,
        int iterations)
    {
        IBenchmark benchmarkToRun;

        switch (benchmarkName)
        {
            case "GetTrackByName":
                Console.WriteLine("Enter track name:");
                var trackName = Console.ReadLine();
                benchmarkToRun = new GetTrackByNameBenchmark(trackName ?? string.Empty);
                break;
            case "GetPlaylistByName":
                Console.WriteLine("Enter Playlist name:");
                var playlistName = Console.ReadLine();
                benchmarkToRun = new GetPlaylistByNameBenchmark(playlistName ?? string.Empty);
                break;
            default:
                throw new ArgumentException("Invalid benchmark name");
        }
        
        Console.WriteLine($"Running benchmark {benchmarkName}...");
        await benchmarkToRun.RunBenchmarkAsync(connectionString, databaseName, collectionName, iterations);
    }
}