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




            while (true) {
                Console.WriteLine("====================");
                Console.WriteLine("Console DDL Extractor");
                Console.WriteLine("====================");
                Console.WriteLine("MENU\n1.Server Side.\n2.Locally");
                string choose = Console.ReadLine();
                switch (choose) {

                    case "2":
                        Console.Clear();
                        
                        Console.WriteLine("Starting File Analyzer in Console Mode...");
                        Console.WriteLine("Write file path:");
                        string path = Console.ReadLine();
                        await showMetaData(path);
                            
                            break;
                    case "1":
                        Console.Clear();
                        var client = new MetadataServiceClient(MetadataServiceClient.EndpointConfiguration.WSHttpBinding_IMetadataService, "https://localhost:5001/MetadataService/WSHttps");
                        //Console.WriteLine("Available plugins:");

                        //var loader = new PluginLoader();
                        //loader.LoadPlugins();
                        //var plugins = loader.Plugins.ToList();

                        //for (int i = 0; i < plugins.Count; i++) {
                        //    Console.WriteLine($"{i + 1}: {plugins[i].Name}");
                        //}
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

                            string result = await client.ProcessMetadataResultAsync(fileBytes, selection-1);
                            Console.WriteLine($"Processed Metadata: {result.ToString()}");
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

        public static async Task showMetaData(string file) {
            if (!File.Exists(file)) {
                Console.WriteLine("Invalid file Path!");
            } else {
                var loader = new PluginLoader();
                loader.LoadPlugins();

                Console.WriteLine("Available plugins:");
                var plugins = loader.Plugins.ToList();
                for (int i = 0; i < plugins.Count; i++) {
                    Console.WriteLine($"{i + 1}: {plugins[i].Name}");
                }

                Console.Write("Select a plugin by number: ");
                int selection = int.Parse(Console.ReadLine());

                if (selection > 0 && selection <= plugins.Count) {
                    var selectedPlugin = plugins[selection - 1];
                    Console.WriteLine($"Using plugin: {selectedPlugin.Name}");

                    var obj = await selectedPlugin.Read(file);
                    string result = obj.ToString();
                    Console.WriteLine($"Result: {result}");
                } else {
                    Console.WriteLine("Invalid selection.");
                }
            }
        }

    }
        
    }
