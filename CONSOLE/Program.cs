using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CoreLibrary;

using LoggingLibrary;
using MetaDataLibrary;
using MetadataService;
using MODEL;


namespace ConsoleApp
{
    class Program {
        private readonly ILogger _logger;
        
        public static async Task<int> Main(string[] args) {
            while (true) {

                Console.WriteLine("====================");
                Console.WriteLine("Console DDL Extractor");
                Console.WriteLine("====================");
                Console.WriteLine("MENU\n1.Server Side.\n2.Locally");
                string choose = Console.ReadLine();
                switch (choose) {

                    case "2":
                        Console.Clear();
                        
                        Console.WriteLine("Starting DLL Analyzer in Console Mode...");
                        Console.WriteLine("Write dll file path:");
                        string path = Console.ReadLine();
                        await showMetaData(path);
                            
                            break;
                    case "1":
                        Console.Clear();
                        var client = new MetadataServiceClient(MetadataServiceClient.EndpointConfiguration.WSHttpBinding_IMetadataService, "https://localhost:5001/MetadataService/WSHttps");
                        Console.WriteLine("Write dll file path:");
                        string dllPath = Console.ReadLine();

                        if (!File.Exists(dllPath)) {
                            Console.Clear();
                            Console.WriteLine($"File {dllPath} not found.");
                            break;
                        }

                        byte[] fileBytes = File.ReadAllBytes(dllPath);

                        try {

                            string result = await client.ProcessMetadataResultAsync(fileBytes);
                            Console.WriteLine($"Processed Metadata: {result}");
                        } catch (Exception ex) {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid Choice! Try Again.");
                        break;
            }


            }



        }
        
        public static async Task showMetaData(string filePath) {
            var logger = new Logger();
            await logger.LogStartAsync();

            try {
                MetadataExtractor metaData = new MetadataExtractor(logger);
                AssemblyInfo assemblyInfo = await metaData.ExtractMetadataAsync(filePath);

                if (assemblyInfo.Types.Count == 0)
                    throw new Exception("File is Empty");
                foreach (var type in assemblyInfo.Types) {
                    Console.WriteLine($"Type: {type.TypeName}");
                    foreach (var method in type.Methods) {
                        Console.WriteLine($"  Method: {method.MethodName}");
                    }
                }
                Console.WriteLine("Do you want to save this Metadata to XML File? [Y|N]");
                try {
                    string choice = Console.ReadLine();

                    switch (choice) {
                        case "y":
                        case "Y":
                            SaveTo save = new SaveTo(logger);
                            save.XmlFile(metaData.getDataList());
                            Console.WriteLine("XML file succesfully saved!");

                            break;
                        case "n":
                        case "N":
                            Console.WriteLine("Ok. Goodbye");
                            break;
                        default:
                            Console.WriteLine("Invalid Input");
                            break;

                    }
                } catch {
                    Console.WriteLine("Invalid Input");
                }


            } catch (Exception se) {
                Console.Clear();
                await logger.LogErrorAsync();
                Console.WriteLine($"Error in reading the dll. {se.Message}");
            }

        }

    }
        
    }
