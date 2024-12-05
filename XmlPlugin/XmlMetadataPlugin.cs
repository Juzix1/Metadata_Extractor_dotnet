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
        private List<DataList> dataDictionary = new List<DataList>();
        public async Task<object> Read(string sourcePath) {
            return await Task.Run(() => ExtractMetadata(sourcePath));
        }

        private object ExtractMetadata(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    throw new FileNotFoundException("The file was not found", filePath);
                }

                dataDictionary.Clear();

                using (XmlReader reader = XmlReader.Create(filePath)) {
                    while (reader.Read()) {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string elementName = reader.Name;
                            string elementValue = string.Empty;

                            if (reader.IsEmptyElement)
                            {
                                elementValue = "(empty)";
                            } else if (reader.Read())
                              {
                                if (reader.NodeType == XmlNodeType.Text) {
                                    elementValue = reader.Value;
                                }
                            }
                            dataDictionary.Add(new DataList {
                                type = elementName,
                                name = elementValue
                            });
                        }
                    }
                }

                return this;
            } catch (Exception ex) {
                Console.WriteLine($"Error reading XML: {ex.Message}");
                return null;
            }
        }

        public override string ToString() {
            if (dataDictionary.Count == 0) {
                return "No data available.";
            }

            StringBuilder sb = new StringBuilder();

            foreach (var data in dataDictionary) {
                sb.AppendLine($"{data.type}: {data.name}");
            }

            return sb.ToString();
        }
    }
}
