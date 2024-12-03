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

            var client = new MetadataServiceClient(MetadataServiceClient.EndpointConfiguration.WSHttpBinding_IMetadataService, "https://localhost:5001/MetadataService/WSHttps");
            string dllPath = @"C:\Users\kapik\Documents\Visual Studio 2022\MetaDataExtractor\Metadata_Extractor_dotnet\CONSOLE\bin\Debug\net6.0\Model.dll";

            if (!File.Exists(dllPath)) {
                Console.WriteLine($"File {dllPath} not found.");
                return 0;
            }

            byte[] fileBytes = File.ReadAllBytes(dllPath);

            try {
                string result = await client.ProcessMetadataResultAsync(fileBytes);
                Console.WriteLine($"Processed Metadata: {result}");
            } catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
            }
        
    



            //var client = new EchoServiceClient(EchoServiceClient.EndpointConfiguration.WSHttpBinding_IEchoService, "https://localhost:5001/EchoService/WSHttps");

            //var simpleResult = await client.EchoAsync("Hello");
            //Console.WriteLine(simpleResult);

            //var msg = new EchoMessage() { Text = "Hello2" };
            //var msgResult = await client.ComplexEchoAsync(msg);
            //Console.WriteLine(msgResult);
            //return 0;
            return 0;
        }/*
        
        public static int Main(string[] args) {
            if (args.Length > 0) {
                Console.WriteLine("Starting DLL Analyzer in Console Mode...");
                showMetaData(args[0]);
                return 0;
            } else {
                Console.WriteLine("Invalid File path, try again");
                return 0;
            }


        }
        
        public static async Task showMetaData(string filePath) {
            var logger = new Logger();
            await logger.LogStartAsync();

            try {
                MetadataExtractor metaData = new MetadataExtractor(logger);
                AssemblyInfo assemblyInfo = await metaData.ExtractMetadataAsync(filePath);
                
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
                await logger.LogErrorAsync();
                Console.WriteLine($"Error in reading the dll file {se.Message}");
            }

        }*/

    }
        
    }
