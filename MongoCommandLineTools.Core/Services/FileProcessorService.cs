using System.Collections.Concurrent;
using System.Text.Json;
using MongoCommandLineTools.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCommandLineTools.Core.Services;

public class FileProcessor(IMongoDbService mongoDbService) : IFileProcessor
{
    private readonly IMongoDbService _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
    public ConcurrentQueue<string> ProcessedFiles { get; } = new();
    
    public async Task ProcessJsonFilesAsync(string directoryPath, string connectionString, string databaseName, string collectionName)
    {
        var jsonFiles = Directory.GetFiles(directoryPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Console.WriteLine("No JSON files found in the directory.");
            return;
        }

        var tasks = jsonFiles.Select(async filePath =>
        {
            var fileName = Path.GetFileName(filePath);
            var filter = Builders<BsonDocument>.Filter.Eq("fileName", fileName);
            var processedFilesCollection = await _mongoDbService.GetCollectionAsync("processed_files");

            if (await _mongoDbService.CollectionExistsAsync(collectionName))
            {
                Console.WriteLine($"Collection '{collectionName}' does not exist in the database '{databaseName}'.");
                return;
            }

            if (await processedFilesCollection.Find(filter).AnyAsync())
            {
                Console.WriteLine($"File '{fileName}' has already been processed. Skipping.");
                return;
            }

            Console.WriteLine($"Processing file: {filePath}");
            try
            {
                await using var fileStream = File.OpenRead(filePath);
                using var jsonDocument = await JsonDocument.ParseAsync(fileStream);

                if (!jsonDocument.RootElement.TryGetProperty("playlists", out var playlistsElement))
                {
                    Console.WriteLine("No playlists found in the JSON file.");
                    return;
                }

                var bsonDocuments = playlistsElement.EnumerateArray()
                    .Select(ConvertToBsonDocument)
                    .ToList();

                await _mongoDbService.BulkInsertAsync(collectionName, bsonDocuments);

                Console.WriteLine($"Inserted {bsonDocuments.Count} playlists from file {Path.GetFileName(filePath)}.");

                var processedFileDocument = new BsonDocument { { "fileName", fileName } };
                await processedFilesCollection.InsertOneAsync(processedFileDocument);
                Console.WriteLine($"File '{fileName}' marked as processed.");

                ProcessedFiles.Enqueue(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);
    }

    private static BsonDocument ConvertToBsonDocument(JsonElement element)
    {
        var jsonString = element.GetRawText();
        return BsonDocument.Parse(jsonString);
    }
}