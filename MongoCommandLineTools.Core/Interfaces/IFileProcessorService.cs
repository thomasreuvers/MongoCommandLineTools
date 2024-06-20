using System.Collections.Concurrent;

namespace MongoCommandLineTools.Core.Interfaces;

public interface IFileProcessorService
{
    Task ProcessJsonFilesAsync(string directoryPath, string connectionString, string databaseName, string collectionName);
    ConcurrentQueue<string> ProcessedFiles { get; }
}