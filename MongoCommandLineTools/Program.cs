using Microsoft.Extensions.DependencyInjection;
using MongoCommandLineTools.Core.Commands;
using MongoCommandLineTools.Core.Interfaces;
using MongoCommandLineTools.Core.Services;
using MongoDB.Driver;

namespace MongoCommandLineTools;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        var serviceProvider = ConfigureServices();

        var commandName = args[0].ToLower();

        var commandHandler = GetCommandHandler(serviceProvider, commandName);
        if (commandHandler == null)
        {
            Console.WriteLine($"Command '{commandName}' not recognized.");
            ShowUsage();
            return;
        }

        await commandHandler.ExecuteAsync(args[1..]);
    }
    
    private static void ShowUsage()
    {
        Console.WriteLine("Usage: MongoDBJsonImporter <command> [args]");
        Console.WriteLine("Commands:");
        Console.WriteLine("  import <connectionString> <databaseName> <collectionName> <directoryPath>");
        Console.WriteLine("  benchmark <benchmarkName> <connectionString> <databaseName> <collectionName> <iterations>");
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // Register MongoDB client and collections
        serviceCollection.AddSingleton<IMongoClient>(new MongoClient());
        serviceCollection.AddSingleton<IFileProcessorService, FileProcessorService>();
        serviceCollection.AddSingleton<IBenchmarkProcessorService, BenchmarkProcessorService>();

        // Register commands
        serviceCollection.AddSingleton<ICommandHandler, ImportCommandHandler>();
        serviceCollection.AddSingleton<ICommandHandler, BenchmarkCommandHandler>();

        return serviceCollection.BuildServiceProvider();
    }

    private static ICommandHandler? GetCommandHandler(IServiceProvider serviceProvider, string commandName)
    {
        var handlers = serviceProvider.GetServices<ICommandHandler>();
        return handlers.FirstOrDefault(h =>
            h.GetType().Name.Equals($"{commandName}CommandHandler", StringComparison.CurrentCultureIgnoreCase)
        );
    }
}