using LoggingLibrary;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MetaDataLibrary {
    public class Logger : ILogger{
        private readonly String path = "./logs/log.txt";

        public Logger() {
            Directory.CreateDirectory("./logs");
        }


        private async Task AppendLogAsync(string message) {
            try {
                string logEntry = $"{DateTime.Now:HH:mm:ss tt} [INFO]| {message}\n";
                await File.AppendAllTextAsync(path, logEntry);
            } catch (Exception ex) {

                Console.WriteLine($"Error logging message: {ex.Message}");
            }
        }


        public async Task LogStartAsync() => await AppendLogAsync("DLL search started.");
        public async Task LogFoundAsync(string file) => await AppendLogAsync($" File ({file}) found.");
        public async Task LogErrorAsync() => await AppendLogAsync("Error during DLL search.");
        public async Task LogStartReadAsync() => await AppendLogAsync("Starting to read the DLL file.");

        public async Task LogFinishAsync() => await AppendLogAsync("Finished reading the DLL file.");
        public async Task LogStartXmlSave() => await AppendLogAsync("Starting to save file");
        public async Task LogErrorXmlSave(string message) => await AppendLogAsync("Failed to save XML file. Error: "+message);
        public async Task LogErrorDBSave(string message) => await AppendLogAsync("Failed to save to the MicrosoftSQL Databse. Error: " + message);


    }
}


