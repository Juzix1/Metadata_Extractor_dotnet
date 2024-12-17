using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreLibrary;

using LoggingLibrary;
using MetaDataLibrary;
using MetadataService;
using MODEL;
using MODEL.Plugins;


namespace ConsoleApp
{
    class Program {
        private readonly ILogger _logger;
        
        public static async Task<int> Main(string[] args) {


            string path = "./logs/metadata.xml";
            PluginLoader loader = new PluginLoader();
            AssemblyInfo assemblyInfo = null;

            while (true) {


                Console.WriteLine("====================");
                Console.WriteLine("Console DDL Extractor");
                Console.WriteLine("====================");
                Console.WriteLine("MENU\n1.Server Side.\n2.Locally\n3.Save file");
                string choose = Console.ReadLine();
                switch (choose) {

                    case "2":
                        Console.Clear();
                        
                        Console.WriteLine("Starting File Analyzer in Console Mode...");
                        assemblyInfo = await showMetaData(loader);


                        break;
                    case "1":
                        try {
                            Console.Clear();
                            var client = new MetadataServiceClient(MetadataServiceClient.EndpointConfiguration.WSHttpBinding_IMetadataService, "https://localhost:5001/MetadataService/WSHttps");
                        Console.WriteLine(await client.ShowAvailableAddonsAsync());

                        Console.Write("Select a plugin by number: ");
                        
                            int selection = int.Parse(Console.ReadLine());
                            

                        Console.WriteLine("Write file path:");
                        string dllPath = Console.ReadLine();

                        if (!File.Exists(dllPath)) {
                            Console.Clear();
                            Console.WriteLine($"File {dllPath} not found.");
                            break;
                        }

                        byte[] fileBytes = File.ReadAllBytes(dllPath);

                        try {

                            var result = await client.ProcessMetadataResultAsync(fileBytes, selection-1);
                            Console.WriteLine($"Processed Metadata: {result.ToString()}");
                        } catch (Exception ex) {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        } catch (Exception e) {
                            Console.WriteLine(e.Message);
                        }
                        break;
                        case "3":
                        Console.Clear();
                        if (assemblyInfo == null) {
                            Console.WriteLine("You didn't selected file");

                        } else {
                            Console.WriteLine("Select save format.\n1.To XML File\n2.to Database");
                            int selection2 = int.Parse(Console.ReadLine());
                            try {
                                SaveTo save = new(new Logger());
                                var dataList = new List<DataList>();
                                foreach (var type in assemblyInfo.Types) {
                                        dataList.Add(new DataList { type = "Type", name = type.TypeName });

                                    foreach (var method in type.Methods) {    
                                            dataList.Add(new DataList { type = "Method", name = method.MethodName });
                                    }
                            }



                            switch (selection2) {
                                
                                case 1:
                                        save.XmlFile(assemblyInfo.Name,dataList,path);
                                    break;
                                case 2:

                                        save.MSDatabase(assemblyInfo);
                                        break;

                                default:
                                    Console.Clear();
                                    Console.WriteLine("Wrong Option");
                                    break;
                            }
                                
                            } catch (Exception ex) {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid Choice! Try Again.");
                        break;

            }


            }



        }


        public static async Task<AssemblyInfo> showMetaData(PluginLoader loader) {
            loader = new PluginLoader();
            var assemblyInfo = new AssemblyInfo();
            try {
                loader.LoadPlugins();
                //TOFIX
                Console.WriteLine("Available plugins:");
                var plugins = loader.Plugins.ToList();
                for (int i = 0; i < plugins.Count; i++) {
                    Console.WriteLine($"{i + 1}: {plugins[i].Name}");
                }

                Console.Write("Select a plugin by number: ");
                int selection = int.Parse(Console.ReadLine());


                Console.WriteLine("Write file path:");
                string file = Console.ReadLine();

                if (selection > 0 && selection <= plugins.Count) {
                    var selectedPlugin = plugins[selection - 1];
                    Console.WriteLine($"Using plugin: {selectedPlugin.Name}");

                    var obj = await selectedPlugin.Read(file);
                    string result = obj.ToString();
                    assemblyInfo = selectedPlugin.GetAssemblyInfo();
                    assemblyInfo.Name = file;

                    Console.WriteLine($"Result: {result}");
                    return assemblyInfo;
                } else {
                    Console.WriteLine("Invalid selection.");
                    return null;
                }

            } catch (Exception ex) {
                Console.WriteLine($"Reading Failed: {ex.Message}");
                return null;
            }

        }

        private static string CleanTypeName(string typeName) {
            if (string.IsNullOrEmpty(typeName))
                return string.Empty;

            var cleanName = typeName.Replace("<", string.Empty).Replace(">", string.Empty);

            return cleanName;
        }
    }
}

    
        
    

