using CoreLibrary;
using LoggingLibrary;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MODEL {

    public class DataList {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class SaveTo {
        private readonly ILogger _logger;
        private readonly string path = "./logs/metadata.xml";
        private readonly string _connectionString = "Data Source=localhost;Initial Catalog=DLLDatabase;Integrated Security=True;Trust Server Certificate=True;MultipleActiveResultSets=True";

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

                textWriter.WriteStartDocument();
                textWriter.WriteStartElement(rootElementName);

                foreach (var data in dataList) {
                    textWriter.WriteElementString(data.type, data.name);
                }

                textWriter.WriteEndElement();
                textWriter.WriteEndDocument();
            }

            _logger.LogFinishAsync();
        }

        public void MSDatabase(AssemblyInfo assemblyInfo) {
            if (assemblyInfo?.Types == null || assemblyInfo.Types.Count == 0) {
                _logger.LogErrorDBSave("No data to save to the database.");
                return;
            }

            try {
                using (SqlConnection connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    Console.WriteLine("Database connection opened.");

                    using (SqlTransaction transaction = connection.BeginTransaction()) {
                        try {
                            string truncateTypesQuery = "TRUNCATE TABLE Types";
                            string truncateMethodsQuery = "TRUNCATE TABLE Methods";

                            using (SqlCommand truncateTypesCommand = new SqlCommand(truncateTypesQuery, connection, transaction)) {
                                truncateTypesCommand.ExecuteNonQuery();
                            }
                            using (SqlCommand truncateMethodsCommand = new SqlCommand(truncateMethodsQuery, connection, transaction)) {
                                truncateMethodsCommand.ExecuteNonQuery();
                            }

                            Dictionary<string, int> typeIds = new Dictionary<string, int>();

                            foreach (var type in assemblyInfo.Types) {
                                string selectTypeQuery = "SELECT TypeId FROM Types WHERE TypeName = @TypeName";
                                using (SqlCommand selectTypeCommand = new SqlCommand(selectTypeQuery, connection, transaction)) {
                                    selectTypeCommand.Parameters.AddWithValue("@TypeName", type.TypeName);
                                    object existingTypeId = selectTypeCommand.ExecuteScalar();

                                    if (existingTypeId == null) {
                                        string insertTypeQuery = "INSERT INTO Types (TypeName) OUTPUT INSERTED.TypeId VALUES (@TypeName)";
                                        using (SqlCommand insertTypeCommand = new SqlCommand(insertTypeQuery, connection, transaction)) {
                                            insertTypeCommand.Parameters.AddWithValue("@TypeName", type.TypeName);
                                            int newTypeId = (int)insertTypeCommand.ExecuteScalar();
                                            typeIds[type.TypeName] = newTypeId;
                                            Console.WriteLine($"Inserted Type: {type.TypeName} with TypeId: {newTypeId}");
                                        }
                                    } else {
                                        int existingId = Convert.ToInt32(existingTypeId);
                                        typeIds[type.TypeName] = existingId;
                                        Console.WriteLine($"Type already exists: {type.TypeName} with TypeId: {existingId}");
                                    }
                                }
                            }

                            foreach (var type in assemblyInfo.Types) {
                                if (type.Methods != null) {
                                    foreach (var method in type.Methods) {
                                        if (typeIds.ContainsKey(type.TypeName)) {
                                            int typeId = typeIds[type.TypeName];

                                            string selectMethodQuery = "SELECT MethodId FROM Methods WHERE MethodName = @MethodName AND TypeId = @TypeId";
                                            using (SqlCommand selectMethodCommand = new SqlCommand(selectMethodQuery, connection, transaction)) {
                                                selectMethodCommand.Parameters.AddWithValue("@MethodName", method.MethodName);
                                                selectMethodCommand.Parameters.AddWithValue("@TypeId", typeId);
                                                object existingMethod = selectMethodCommand.ExecuteScalar();

                                                if (existingMethod == null) {
                                                    string insertMethodQuery = "INSERT INTO Methods (MethodName, TypeId) VALUES (@MethodName, @TypeId)";
                                                    using (SqlCommand insertMethodCommand = new SqlCommand(insertMethodQuery, connection, transaction)) {
                                                        insertMethodCommand.Parameters.AddWithValue("@MethodName", method.MethodName);
                                                        insertMethodCommand.Parameters.AddWithValue("@TypeId", typeId);
                                                        insertMethodCommand.ExecuteNonQuery();
                                                        Console.WriteLine($"Inserted Method: {method.MethodName} for TypeId: {typeId}");
                                                    }
                                                } else {
                                                    Console.WriteLine($"Method already exists: {method.MethodName}");
                                                }
                                            }
                                        } else {
                                            Console.WriteLine($"Type not found for Method: {method.MethodName}");
                                        }
                                    }
                                }
                            }

                            transaction.Commit();
                            _logger.LogFinishAsync();
                        } catch (Exception ex) {
                            transaction.Rollback();
                            _logger.LogErrorDBSave($"Error saving data to the database: {ex.Message}");
                        }
                    }
                }
            } catch (Exception ex) {
                _logger.LogErrorDBSave($"Error connecting to the database: {ex.Message}");
            }
        }


    }

}
