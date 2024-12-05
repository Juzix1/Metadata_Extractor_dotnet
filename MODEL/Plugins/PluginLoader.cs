using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MODEL.Plugins {
    public class PluginLoader {
        private readonly List<IMetadataPlugin> _plugins = new List<IMetadataPlugin>();

        public IEnumerable<IMetadataPlugin> Plugins => _plugins;

        public void LoadPlugins() {

            string rootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..",".."));
            string pluginFolder = Path.Combine(rootPath, "Plugins");

            if (!Directory.Exists(pluginFolder)) {
                throw new DirectoryNotFoundException($"Plugin folder not found: {pluginFolder}");
            }

            var pluginFiles = Directory.GetFiles(pluginFolder);

            foreach (var file in pluginFiles) {
                try {
                    var assembly = Assembly.LoadFrom(file);

                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(IMetadataPlugin).IsAssignableFrom(t) && !t.IsAbstract);

                    foreach (var type in pluginTypes) {
                        var pluginInstance = (IMetadataPlugin)Activator.CreateInstance(type);
                        _plugins.Add(pluginInstance);
                    }
                } catch (Exception ex) {
                    Debug.WriteLine($"Failed to load plugin from {file}: {ex.Message}");
                }
            }
        }
    }
}
