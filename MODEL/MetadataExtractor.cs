using CoreLibrary;
using LoggingLibrary;
using MODEL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MetaDataLibrary
{
    
    public class MetadataExtractor
    {
        private readonly ILogger _logger;
        List<DataList> dataDictionary = new List<DataList>();
        public MetadataExtractor(ILogger logger) {
            _logger = logger;
        }

        public List<DataList> getDataList()
        {
            return dataDictionary;
        }
        public async Task<AssemblyInfo> ExtractMetadataAsync(string filePath) {

            var assemblyInfo = new AssemblyInfo();

            await _logger.LogStartAsync();

            try {
                dataDictionary.Clear();
                Assembly assembly = Assembly.LoadFile(filePath);
                assemblyInfo.Name = assembly.FullName;

                foreach (var type in assembly.GetTypes()) {
                    var typeInfo = new CoreLibrary.TypeInfo { TypeName = type.FullName };
                    assemblyInfo.Types.Add(typeInfo);
                    dataDictionary.Add(new DataList { type = "Type", name = type.FullName });

                    foreach (var method in type.GetMethods()) {
                        var methodInfo = new CoreLibrary.MethodInfo { MethodName = method.Name };
                        typeInfo.Methods.Add(methodInfo);
                        dataDictionary.Add(new DataList { type = "Method", name = method.Name });
                        await _logger.LogReadMethod(method.Name);
                    }

                    await _logger.LogReadType(type.FullName);
                }

            }catch (Exception) {
                await _logger.LogErrorAsync();
            }

            return assemblyInfo;
        }
        public async Task<AssemblyInfo> GetDataAsync(string path)
        {
            return await ExtractMetadataAsync(path);

        }
    }
}
