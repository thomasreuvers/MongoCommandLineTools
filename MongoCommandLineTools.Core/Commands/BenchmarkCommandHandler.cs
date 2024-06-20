using MongoCommandLineTools.Core.Interfaces;

namespace MongoCommandLineTools.Core.Commands;

public class BenchmarkCommandHandler(IBenchmarkProcessorService benchmarkProcessorService) : ICommandHandler
{
    public async Task ExecuteAsync(string[] args)
    {
        if (args.Length != 5)
        {
            Console.WriteLine("Usage: benchmark <benchmarkName> <connectionString> <databaseName> <collectionName> <iterations>");
            return;
        }
        
        var benchmarkName = args[0];
        var connectionString = args[1];
        var databaseName = args[2];
        var collectionName = args[3];
        var iterations = args[4];
        
        await benchmarkProcessorService.RunBenchmarkAsync(benchmarkName, connectionString, databaseName, collectionName, int.Parse(iterations));
    }
}