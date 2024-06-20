namespace MongoCommandLineTools.Core.Interfaces;

public interface IBenchmark
{
    Task RunBenchmarkAsync(string connectionString, string databaseName, string collectionName, int iterations);
}