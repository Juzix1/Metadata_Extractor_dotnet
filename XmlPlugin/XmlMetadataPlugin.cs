using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using CoreLibrary;
using MODEL;
using MODEL.Plugins;

namespace XMLMetadataPlugin {
    public class XMLMetadataPlugin : IMetadataPlugin {
        public string Name => "XML Metadata Plugin";
        private List<DataList> _dataDictionary = new List<DataList>();
        AssemblyInfo assemblyInfo;
        public async Task<object> Read(string sourcePath) {
            await Task.Run(() => ExtractMetadataFromFile(sourcePath));
            return this;
        }
        public AssemblyInfo GetAssemblyInfo() {
            return assemblyInfo;
        }

        public async Task<object> ReadStream(Stream stream) {
            return await Task.Run(() => ExtractMetadataFromStream(stream));
        }

        private object ExtractMetadataFromFile(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    throw new FileNotFoundException("The file was not found", filePath);
                }

                _dataDictionary.Clear();

                

                using (var reader = XmlReader.Create(filePath)) {
                    if (reader.ReadState == ReadState.Initial) {
                        Console.WriteLine("Starting XML read...");
                    }

                    ExtractXmlData(reader);
                }

                return this;
            } catch (XmlException xmlEx) {
                Console.WriteLine($"Error reading XML file: {xmlEx.Message}");
                return null;
            } catch (Exception ex) {
                Console.WriteLine($"General error: {ex.Message}");
                return null;
            }
        }

        private object ExtractMetadataFromStream(Stream stream) {
            try {
                if (stream == null || stream.Length == 0) {
                    throw new ArgumentException("Stream is empty or null.");
                }

                _dataDictionary.Clear();
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = XmlReader.Create(stream)) {
                    ExtractXmlData(reader);
                }

                return this;
            } catch (XmlException xmlEx) {
                Console.WriteLine($"Error reading XML stream: {xmlEx.Message}");
                return null;
            } catch (Exception ex) {
                Console.WriteLine($"General error: {ex.Message}");
                return null;
            }
        }

        private void ExtractXmlData(XmlReader reader) {
            try {
                assemblyInfo = new AssemblyInfo();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        if (assemblyInfo.Name == null) {
                            assemblyInfo.Name = reader.Name;
                            _dataDictionary.Add(new DataList { name = "File", type = assemblyInfo.Name });
                           
                        } else {
                            string elementName = reader.Name;
                            var typeInfo = new TypeInfo { TypeName = reader.Name };
                            assemblyInfo.Types.Add(typeInfo);
                            string elementValue = string.Empty;

                            if (reader.IsEmptyElement) {
                                elementValue = "(empty)";
                                typeInfo.Methods.Add(new MethodInfo { MethodName = "" });
                            } else if (reader.Read() && reader.NodeType == XmlNodeType.Text) {
                                elementValue = reader.Value;
                                typeInfo.Methods.Add(new MethodInfo { MethodName = reader.Value });
                            }

                            var datalist = new DataList { type = elementName, name = elementValue };
                            _dataDictionary.Add(datalist);
                        }
                        
                    }
                }
            } catch (XmlException xmlEx) {
                Console.WriteLine($"XML Read Error: {xmlEx.Message}");
            }
        }

        public override string ToString() {
            if (_dataDictionary.Count == 0) {
                return "No data available.";
            }

            StringBuilder sb = new StringBuilder();
            foreach (var data in _dataDictionary) {
                sb.AppendLine(data.name + ": " + data.type);
            }

            return sb.ToString();
        }


    }
}
