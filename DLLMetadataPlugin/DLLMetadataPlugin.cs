using CoreLibrary;
using LoggingLibrary;
using MetaDataLibrary;
using MODEL;
using MODEL.Plugins;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DLLMetadataPlugin {
    public class DLLMetadataPlugin: IMetadataPlugin {
        public string Name => "DLL Metadata Plugin";
        public AssemblyInfo assemblyInfo;
        public async Task<object> Read(string sourcePath) {
            var log = new Logger();
            MetadataExtractor metaData = new MetadataExtractor(log);
            assemblyInfo = await metaData.ExtractMetadataAsync(sourcePath);
            return this;
        }

        public override string ToString() {
            if (assemblyInfo == null || assemblyInfo.Types.Count == 0) {
                return "No data available.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Assembly: {assemblyInfo.Name}");

            foreach (var type in assemblyInfo.Types) {
                sb.AppendLine($"Type: {type.TypeName}");

                foreach (var method in type.Methods) {
                    sb.AppendLine($"  Method: {method.MethodName}");
                }
            }

            return sb.ToString();
        }
    }
}