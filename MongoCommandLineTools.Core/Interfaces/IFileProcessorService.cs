namespace MongoCommandLineTools.Core.Interfaces;

public interface IFileProcessorService
{
    Task ProcessJsonFilesAsync(string connectionString, string databaseName, string collectionName, string directoryPath);
}