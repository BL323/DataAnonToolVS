using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Infrastructure.DataLoading
{
    public static class DataLoader
    {
            public static DataMapper LoadCsv(string fName)
            {
                var init = false;

                var fStream = new FileStream(fName, FileMode.Open);
                TextReader textReader = new StreamReader(fStream);
                var csv = new CsvReader(textReader);


                var fieldNames = new List<string>();
                var fields = new List<List<string>>();

                while (csv.Read())
                {
                    if (!init)
                    {
                        var headers = csv.FieldHeaders;
                        foreach (var header in headers)
                        {
                            fields.Add(new List<string>());
                            fieldNames.Add(header.Trim());
                        }
                        init = true;
                    }

                    for (var fieldNo = 0; fieldNo < fieldNames.Count; fieldNo++)
                    {
                        fields[fieldNo].Add((csv.GetField(fieldNo)).Trim());
                    }
                }

                textReader.Close();
                fStream.Close();

                var relationData = new DataMapper(fieldNames, fields);
                return relationData;
            }

            public static void WriteCsv(string fName, DataMapper relationData)
            {
                var fStream = new FileStream(fName, FileMode.Create);
                var textWriter = new StreamWriter(fStream);
                var csv = new CsvWriter(textWriter);


                //write headers
                foreach (var header in relationData.Headers)
                {
                    csv.WriteField(header);
                }
                csv.NextRecord();

                for (int rowCount = 0; rowCount < relationData.Fields[0].Count; rowCount++)
                {
                    foreach (var field in relationData.Fields)
                    {
                        csv.WriteField(field[rowCount]);
                    }
                    csv.NextRecord();
                }

                textWriter.Close();
                fStream.Close();
            }

            public static DataTable GenerateDataTable(DataMapper relationalData)
            {

                try
                {
                    var dt = new DataTable("TestTabel1");
                    DataColumn column;

                    //dynamically build data table columns
                    for (var index = 0; index < relationalData.Headers.Count; index++)
                    {
                        column = new DataColumn();
                        var type = string.Format("System.{0}", relationalData.Types[index]);
                        column.DataType = System.Type.GetType(type);
                        column.ColumnName = relationalData.Headers[index];
                        dt.Columns.Add(column);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating DataTable: " + ex.InnerException.Message);
                }
            }

            public static void PopulateDataTable(DataMapper rData, ref DataTable dataTable)
            {
                //dynamically populate the table
                try
                {

                    for (var rowIndex = 0; rowIndex < rData.Fields[0].Count; rowIndex++)
                    {
                        var row = dataTable.NewRow();

                        for (var colIndex = 0; colIndex < rData.Headers.Count; colIndex++)
                        {
                            var col = rData.Fields[colIndex];
                            var data = col[rowIndex];
                            row[rData.Headers[colIndex]] = data;
                        }
                        dataTable.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error populating DataTable: " + ex.InnerException.Message);
                }
            }

        }
    }



