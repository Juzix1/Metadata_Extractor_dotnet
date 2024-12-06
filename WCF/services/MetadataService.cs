using CoreLibrary;
using CoreWCF;
using LoggingLibrary;
using MODEL.Plugins;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WCF.services {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MetadataService : IMetadataService {
        public async Task<string> ProcessMetadataResult(byte[] fileBytes, int index) {
            try {
                PluginLoader _pluginLoader = new PluginLoader();
                _pluginLoader.LoadPlugins();
                var plugins = _pluginLoader.Plugins.ToList();

                if (plugins.Count == 0) {
                    throw new Exception("No plugins available to process the file.");
                }

                var plugin = plugins[index];
                using var memoryStream = new MemoryStream(fileBytes);
                var result = await plugin.ReadStream(memoryStream);

                return result?.ToString() ?? "Plugin returned no valid data.";
            } catch (Exception ex) {
                return $"Error: {ex.Message}";
            }
        }
        public string ShowAvailableAddons() {
            try {
                var loader = new PluginLoader();

                loader.LoadPlugins();
                var plugins = loader.Plugins.ToList();
                if (plugins.Count == 0) {
                    return "No plugins available.";
                }
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < plugins.Count; i++) {
                    builder.AppendLine($"{i + 1}: {plugins[i].Name}");
                }
                return builder.ToString();
            } catch (Exception ex) {
                Debug.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            }
    }
}
