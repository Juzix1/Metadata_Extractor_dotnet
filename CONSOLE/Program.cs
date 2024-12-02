using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CoreLibrary;
using LoggingLibrary;
using MetaDataLibrary;
using MODEL;

namespace ConsoleApp
{
    class Program {
        private readonly ILogger _logger;
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

        }

    }
        
    }
