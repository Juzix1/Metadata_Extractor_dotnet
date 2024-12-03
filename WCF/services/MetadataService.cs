using CoreWCF;
using MetaDataLibrary;
using LoggingLibrary;
using ILogger = LoggingLibrary.ILogger;
using System.Diagnostics;
using System.Reflection;
using CoreLibrary;
using Microsoft.Extensions.Logging;
using System.Text;

namespace WCF.services {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MetadataService : IMetadataService {

        public async Task<string> ProcessMetadataResult(byte[] fileBytes) {
            try {
                using var memoryStream = new MemoryStream(fileBytes);

                MetadataExtractor metadataExtractor = new MetadataExtractor(new Logger());
                AssemblyInfo assemblyInfo = await metadataExtractor.ExtractMetadataStream(memoryStream);

                var resultBuilder = new StringBuilder();

                foreach (var type in assemblyInfo.Types) {
                    resultBuilder.AppendLine(type.TypeName);

                    foreach (var method in type.Methods) {
                        resultBuilder.AppendLine($"  {method.MethodName}");
                    }
                }

                string result = resultBuilder.ToString();

                if (string.IsNullOrEmpty(result)) {
                    throw new Exception("No metadata was extracted from DLL.");
                }

                return result;

            } catch (UnauthorizedAccessException ex) {
                return $"Access Denied Error: {ex.Message}";
            } catch (IOException ex) {
                return $"IO Error: {ex.Message}";
            } catch (ReflectionTypeLoadException ex) {
                var loaderExceptions = ex.LoaderExceptions.Select(e => e.Message).ToList();
                throw new Exception("Failed to load assembly types. See logs for details.");
            } catch (Exception ex) {
                return $"Error: {ex.Message}";
            }
        }
    }
}