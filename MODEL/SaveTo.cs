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
        public SaveTo(ILogger logger)
        {
            _logger = logger;
        }

        private readonly String path = "./logs/metadata.xml";
        public void XmlFile(List<DataList> dictionary) {

            XmlTextWriter textWriter = new XmlTextWriter(path, null);

            textWriter.Formatting = Formatting.Indented;
            textWriter.Indentation = 4;

            textWriter.WriteStartDocument();
            textWriter.WriteStartElement("MetaData");
            _logger.LogStartXmlSave();
            var counter = 0;
            foreach (DataList data in dictionary) {

                if (data.type == "Type") {

                    if (counter == 0) {
                        textWriter.WriteStartElement("ClassType");
                        textWriter.WriteAttributeString("Name", data.name);
                    }
                    
                    if (counter == dictionary.Count) {
                        textWriter.WriteEndElement();
                    } else if(counter >0){
                        
                        textWriter.WriteEndElement();
                        textWriter.WriteStartElement("ClassType");
                        textWriter.WriteAttributeString("Name", data.name);

                    }
                    counter++;

                } else if(data.type == "Method") {
                    textWriter.WriteElementString("Method", data.name);
                }
                


            }

            textWriter.WriteEndElement();
            textWriter.WriteEndDocument();
            textWriter.Flush();
            textWriter.Close();


        }
    }
}
