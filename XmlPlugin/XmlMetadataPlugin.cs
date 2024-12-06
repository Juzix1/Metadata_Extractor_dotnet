using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MODEL;
using MODEL.Plugins;

namespace XMLMetadataPlugin {
    public class XMLMetadataPlugin : IMetadataPlugin {
        public string Name => "XML Metadata Plugin";
        private List<DataList> _dataDictionary = new List<DataList>();
        public async Task<object> Read(string sourcePath) {
            return await Task.Run(() => ExtractMetadataFromFile(sourcePath));
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
                    ExtractXmlData(reader);
                }

                return this;
            } catch (Exception ex) {
                Console.WriteLine($"Error reading XML file: {ex.Message}");
                return null;
            }
        }

        private object ExtractMetadataFromStream(Stream stream) {
            try {
                if (stream == null || stream.Length == 0) {
                    throw new ArgumentException("Stream is empty or null.");
                }

                _dataDictionary.Clear();
                stream.Seek(0, SeekOrigin.Begin); // Reset stream position
                using (var reader = XmlReader.Create(stream)) {
                    ExtractXmlData(reader);
                }

                return this;
            } catch (Exception ex) {
                Console.WriteLine($"Error reading XML stream: {ex.Message}");
                return null;
            }
        }

        private void ExtractXmlData(XmlReader reader) {
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    string elementName = reader.Name;
                    string elementValue = string.Empty;

                    if (reader.IsEmptyElement) {
                        elementValue = "(empty)";
                    } else if (reader.Read() && reader.NodeType == XmlNodeType.Text) {
                        elementValue = reader.Value;
                    }

                    _dataDictionary.Add(new DataList {
                        type = elementName,
                        name = elementValue
                    });
                }
            }
        }
        

        public override string ToString() {
            if (_dataDictionary.Count == 0) {
                return "No data available.";
            }

            StringBuilder sb = new StringBuilder();

            foreach (var data in _dataDictionary) {
                sb.AppendLine($"{data.type}: {data.name}");
            }

            return sb.ToString();
        }
    }
}
