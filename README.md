# MongoCommandLineTools

MongoCommandLineTools is a command-line tool for importing JSON data into MongoDB and performing benchmarking operations.

## Getting Started

### Downloading the Latest Release

You can download the latest release of MongoCommandLineTools for your operating system from the [Releases](https://github.com/thomasreuvers/MongoCommandLineTools/releases) page.

### Running the Application

1. **Extract the downloaded archive:**  
   Extract the contents of the downloaded zip file to a directory of your choice.

2. **Open a terminal or command prompt:**  
   Navigate to the directory where you extracted the files.

3. **Run the application with different commands:**

   ```bash
   ./MongoCommandLineTools <command> [args]
   ```

Replace `<command>` with one of the following commands and provide the necessary arguments:

- `import <connectionString> <databaseName> <collectionName> <directoryPath>`: Import JSON files from a directory into MongoDB.
- `benchmark <benchmarkName> <connectionString> <databaseName> <collectionName> <iterations>`: Perform benchmarking operations on MongoDB.

## Commands

### Import Command
The import command allows you to import JSON files into MongoDB.
```bash
./MongoCommandLineTools import <connectionString> <databaseName> <collectionName> <directoryPath>
```
- `<connectionString>`: MongoDB connection string.
- `<databaseName>`: Name of the MongoDB database.
- `<collectionName>:` Name of the MongoDB collection to import data into.
- `<directoryPath>`: Path to the directory containing JSON files to import.

### Benchmark Command
The benchmark command performs benchmarking operations on MongoDB.
```bash
./MongoCommandLineTools benchmark <benchmarkName> <connectionString> <databaseName> <collectionName> <iterations>
```
- `<benchmarkName>`: Name of the benchmarking operation to perform. Possible values are:
  - `All`: Perform all benchmarking operations.
  - `GetPlaylistByNameBenchmark`: Get a playlist by name.
  - `GetPlaylistTracksByPlaylistIdBenchmark`: Get tracks of a playlist by playlist ID.
  - `GetTrackByNameBenchmark`: Get a track by name.
  - `InsertDocumentBenchmark`: Insert a document.
  - `UpdateDocumentBenchmark`: Update a document.
  - `DeleteDocumentBenchmark`: Delete a document.
- `<connectionString>`: MongoDB connection string.
- `<databaseName>`: Name of the MongoDB database.
- `<collectionName>`: Name of the MongoDB collection to perform benchmarking on.
- `<iterations>`: Number of iterations for benchmarking operations.
