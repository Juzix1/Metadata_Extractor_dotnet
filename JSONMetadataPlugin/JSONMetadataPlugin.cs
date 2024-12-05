

using MODEL.Plugins;

namespace JSONMetadataPlugin {
    public class JSONMetadataPlugin: IMetadataPlugin {

        public string Name => "JSON Metadata Plugin";

        public async Task<object> Read(string sourcePath) {
            if (!File.Exists(sourcePath)) {
                throw new FileNotFoundException($"File not found: {sourcePath}");
            }
            return File.ReadAllText(sourcePath);
        }

    }
}
