namespace MongoCommandLineTools.Core.Interfaces;

public interface IBenchmarkProcessorService
{
    Task RunBenchmarkAsync(string benchmarkName, string connectionString, string databaseName, string collectionName, int iterations);
}