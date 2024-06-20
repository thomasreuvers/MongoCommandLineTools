﻿using System.Collections.Concurrent;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBJsonImporter;

class Program
{
        private static async Task Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: MongoDBJsonImporter <connectionString> <databaseName> <collectionName> <directoryPath>");
                return;
            }

            var connectionString = args[0];
            var databaseName = args[1];
            var collectionName = args[2];
            var directoryPath = args[3];

            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            {
                Console.WriteLine("Invalid directory path.");
                return;
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var processedFilesCollection = database.GetCollection<BsonDocument>("processed_files");

            var jsonFiles = Directory.GetFiles(directoryPath, "*.json");

            if (jsonFiles.Length == 0)
            {
                Console.WriteLine("No JSON files found in the directory.");
                return;
            }

            // Use ConcurrentQueue to handle processed files
            var processedFiles = new ConcurrentQueue<string>();

            // Use SemaphoreSlim to limit concurrent tasks
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            
            var tasks = jsonFiles.Select(async filePath =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await ProcessJsonFileAsync(filePath, collection, processedFilesCollection, processedFiles);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private static async Task ProcessJsonFileAsync(string filePath, IMongoCollection<BsonDocument> collection, IMongoCollection<BsonDocument> processedFilesCollection, ConcurrentQueue<string> processedFiles)
        {
            var fileName = Path.GetFileName(filePath);
            var filter = Builders<BsonDocument>.Filter.Eq("fileName", fileName);

            // Check if the file has already been processed
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

                // Use BulkWriteModel for batch inserts
                var writeModels = bsonDocuments.Select(doc => new InsertOneModel<BsonDocument>(doc)).ToList();

                // Perform bulk insert
                await collection.BulkWriteAsync(writeModels, new BulkWriteOptions { IsOrdered = false });

                Console.WriteLine($"Inserted {bsonDocuments.Count} playlists from file {Path.GetFileName(filePath)}.");

                var processedFileDocument = new BsonDocument { { "fileName", fileName } };
                await processedFilesCollection.InsertOneAsync(processedFileDocument);
                Console.WriteLine($"File '{fileName}' marked as processed.");

                processedFiles.Enqueue(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
            }
        }

        private static BsonDocument ConvertToBsonDocument(JsonElement element)
        {
            var jsonString = element.GetRawText();
            return BsonDocument.Parse(jsonString);
        }
}