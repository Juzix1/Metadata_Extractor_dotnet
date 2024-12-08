using CoreLibrary;
using LoggingLibrary;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MODEL {

    public class  DataList
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class SaveTo {
        private readonly ILogger _logger;
        private readonly string path = "./logs/metadata.xml";

        public SaveTo(ILogger logger) {
            _logger = logger;
        }

        public void XmlFile(string rootElementName, List<DataList> dataList, string path) {
            if (string.IsNullOrWhiteSpace(rootElementName) || dataList == null || dataList.Count == 0) {
                _logger.LogErrorXmlSave("Invalid data provided for XML file generation.");
                return;
            }

            using (XmlTextWriter textWriter = new XmlTextWriter(path, null)) {
                textWriter.Formatting = Formatting.Indented;
                textWriter.Indentation = 4;

                // Start XML Document
                textWriter.WriteStartDocument();
                textWriter.WriteStartElement(rootElementName); // Root element

                // Write each DataList item as a tag and value
                foreach (var data in dataList) {
                    textWriter.WriteElementString(data.type, data.name);
                }

                // Close the root element
                textWriter.WriteEndElement();
                textWriter.WriteEndDocument();
            }

            _logger.LogFinishAsync();
        }
    }
}