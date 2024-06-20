namespace MongoCommandLineTools.Core.Interfaces;

public interface ICommandHandler
{
    Task ExecuteAsync(string[] args);
}