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
            try {
                    using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                var data = memoryStream.ToArray();
                if (data == null || data.Length == 0) {
                    throw new Exception("Stream is empty or contains invalid data.");
                }

            
                Assembly assembly = Assembly.Load(data);
                return ExtractMetadata(assembly);
            } catch (BadImageFormatException) {
                throw new Exception("The stream does not contain a valid .NET assembly.");
            }
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

        public AssemblyInfo GetAssemblyInfo() {
            return assemblyInfo;
        }
        public override string ToString() {
            StringBuilder sb = new();
            sb.AppendLine($"\nAssembly: {Name}");

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