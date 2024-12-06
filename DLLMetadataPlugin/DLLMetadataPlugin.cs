using CoreLibrary;
using LoggingLibrary;
using MetaDataLibrary;
using MODEL;
using MODEL.Plugins;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using MethodInfo = CoreLibrary.MethodInfo;
using TypeInfo = CoreLibrary.TypeInfo;

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
        public async Task<object> ReadStream(Stream stream) {

            var log = new Logger();
            MetadataExtractor metaData = new MetadataExtractor(log);
            assemblyInfo = await metaData.ExtractMetadataStream(stream);
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
        private AssemblyInfo ExtractMetadata(Assembly assembly) {
            var assemblyInfo = new AssemblyInfo { Name = assembly.FullName };
            foreach (var type in assembly.GetTypes()) {
                var typeInfo = new TypeInfo { TypeName = type.FullName };
                assemblyInfo.Types.Add(typeInfo);

                foreach (var method in type.GetMethods()) {
                    typeInfo.Methods.Add(new MethodInfo { MethodName = method.Name });
                }
            }
            return assemblyInfo;
        }
    }
}