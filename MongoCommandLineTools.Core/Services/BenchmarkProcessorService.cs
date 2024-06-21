using MongoCommandLineTools.Core.Benchmarks;
using MongoCommandLineTools.Core.Constants;
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
        var benchmarksToRun = new List<IBenchmark>();

        switch (benchmarkName.Trim().ToUpper())
        {
            case BenchmarkConstants.GetTrackByName:
                Console.WriteLine("Enter track name:");
                var trackName = Console.ReadLine();
                benchmarksToRun.Add(new GetTrackByNameBenchmark(trackName ?? string.Empty));
                Console.Clear();
                break;
            case BenchmarkConstants.GetPlaylistByName:
                Console.WriteLine("Enter Playlist name:");
                var playlistName = Console.ReadLine();
                benchmarksToRun.Add(new GetPlaylistByNameBenchmark(playlistName ?? string.Empty));
                Console.Clear();
                break;
            case BenchmarkConstants.GetPlaylistTracksByPlaylistId:
                Console.WriteLine("Enter Playlist ID:");
                var playlistId = Console.ReadLine();
                benchmarksToRun.Add(new GetPlaylistTracksByPlaylistIdBenchmark(playlistId ?? string.Empty));
                Console.Clear();
                break;
            case BenchmarkConstants.InsertDocument:
                benchmarksToRun.Add(new InsertDocumentBenchmark());
                Console.Clear();
                break;
            case BenchmarkConstants.UpdateDocument:
                benchmarksToRun.Add(new UpdateDocumentBenchmark());
                Console.Clear();
                break;
            case BenchmarkConstants.DeleteDocument:
                benchmarksToRun.Add(new DeleteDocumentBenchmark());
                Console.Clear();
                break;
            case BenchmarkConstants.All:
                Console.WriteLine("Enter track name:");
                trackName = Console.ReadLine();
                Console.Clear();
                
                Console.WriteLine("Enter Playlist name:");
                playlistName = Console.ReadLine();
                Console.Clear();
                
                Console.WriteLine("Enter Playlist ID:");
                playlistId = Console.ReadLine();
                Console.Clear();
                
                benchmarksToRun.Add(new GetTrackByNameBenchmark(trackName ?? string.Empty));
                benchmarksToRun.Add(new GetPlaylistByNameBenchmark(playlistName ?? string.Empty));
                benchmarksToRun.Add(new GetPlaylistTracksByPlaylistIdBenchmark(playlistId ?? string.Empty));
                benchmarksToRun.Add(new InsertDocumentBenchmark());
                benchmarksToRun.Add(new UpdateDocumentBenchmark());
                benchmarksToRun.Add(new DeleteDocumentBenchmark());
                break;
            default:
                throw new ArgumentException("Invalid benchmark name");
        }
        
        Console.WriteLine($"Running benchmark(s) {benchmarkName}...");
        
        // Run benchmarks sequentially to avoid overloading the database and potential benchmark results skewing
        foreach (var benchmark in benchmarksToRun)
        {
            await benchmark.RunBenchmarkAsync(connectionString, databaseName, collectionName, iterations);
        }
    }
}