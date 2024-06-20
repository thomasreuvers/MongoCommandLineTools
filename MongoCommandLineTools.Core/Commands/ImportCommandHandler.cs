using MongoCommandLineTools.Core.Interfaces;

namespace MongoCommandLineTools.Core.Commands;

public class ImportCommandHandler(IFileProcessorService fileProcessorService) : ICommandHandler
{
    public async Task ExecuteAsync(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: import <connectionString> <databaseName> <collectionName> <directoryPath>");
            return;
        }

        var connectionString = args[0];
        var databaseName = args[1];
        var collectionName = args[2];
        var directoryPath = args[3];

        await fileProcessorService.ProcessJsonFilesAsync(connectionString, databaseName, collectionName, directoryPath);
    }
}