
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;
using Microsoft.Data.SqlClient;
using MODEL;
using MODEL.Plugins;

namespace SQLMetadataPlugin {
    public class SQLMetadataPlugin : IMetadataPlugin {
        public string Name => "SQL Metadata Plugin";
        private List<DataList> _dataDictionary = new List<DataList>();
        private AssemblyInfo assemblyInfo;

        // Database connection string
        private readonly string _connectionString = "Data Source=localhost;Initial Catalog=DLLDatabase;Integrated Security=True;Trust Server Certificate=True;MultipleActiveResultSets=True";

        public async Task<object> Read(string sourcePath) {
            // This method can be used to read metadata from a database table based on sourcePath
            await Task.Run(() => ExtractMetadataFromDatabase(sourcePath));
            return this;
        }

        public AssemblyInfo GetAssemblyInfo() {
            return assemblyInfo;
        }

        public async Task<object> ReadStream(System.IO.Stream stream) {
            //throw new NotImplementedException("Stream reading is not supported for SQL plugin.");
            await Task.Run(() => ExtractMetadataFromDatabase("test"));
            return this;
        }

        private object ExtractMetadataFromDatabase(string tableName) {
            try {
                // Clear previous data
                _dataDictionary.Clear();

                // Create a connection to the SQL database
                using (SqlConnection connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    // Query to get all Types (TypeName)
                    string typeQuery = "SELECT TypeName FROM Types"; // Get all types from the Types table
                    using (SqlCommand typeCommand = new SqlCommand(typeQuery, connection)) {
                        using (SqlDataReader typeReader = typeCommand.ExecuteReader()) {
                            Console.WriteLine("Reading metadata for types...");

                            // Initialize assembly info to store metadata about the types and methods
                            assemblyInfo = new AssemblyInfo();

                            // Fetch all types
                            while (typeReader.Read()) {
                                string typeName = typeReader["TypeName"].ToString();
                                Console.WriteLine($"Type Name: {typeName}");

                                // Now fetch the associated methods for this type
                                // Query to get the methods related to the current type
                                string methodQuery = $"SELECT MethodName FROM Methods WHERE TypeId = (SELECT TypeId FROM Types WHERE TypeName = '{typeName}')";

                                using (SqlCommand methodCommand = new SqlCommand(methodQuery, connection)) {
                                    using (SqlDataReader methodReader = methodCommand.ExecuteReader()) {
                                        while (methodReader.Read()) {
                                            string methodName = methodReader["MethodName"].ToString();
                                            Console.WriteLine($"Method Name: {methodName}");

                                            // Add methods to the assembly info
                                            var typeInfo = new TypeInfo { TypeName = typeName };
                                            typeInfo.Methods.Add(new MethodInfo { MethodName = methodName });

                                            assemblyInfo.Types.Add(typeInfo);

                                            // Add method data to _dataDictionary for potential further use
                                            _dataDictionary.Add(new DataList {
                                                name = methodName,
                                                type = "Method" // Indicating it's a method type (instead of a column)
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return this;
            } catch (SqlException sqlEx) {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                return null;
            } catch (Exception ex) {
                Console.WriteLine($"General error: {ex.Message}");
                return null;
            }
        }

        public override string ToString() {
            if (_dataDictionary.Count == 0) {
                return "No data available.";
            }

            StringBuilder sb = new StringBuilder();
            foreach (var data in _dataDictionary) {
                sb.AppendLine($"{data.name}: {data.type}");
            }

            return sb.ToString();
        }
    }
}