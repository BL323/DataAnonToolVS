using System.Collections.Generic;
using System.Data;
using System.IO;
using CsvHelper;

namespace AnonTool.Infrastructure.DataLoading
{
    public static class DataLoader
    {
        //CsvHelper classes supported by NuGet 3rd party library: http://joshclose.github.io/CsvHelper/ (Last Accessed: 28th April 2015)      
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
                        //First line is attribute headers
                        var headers = csv.FieldHeaders;
                        foreach (var header in headers)
                        {
                            fields.Add(new List<string>());
                            fieldNames.Add(header.Trim());
                        }
                        init = true;
                    }

                    for (var fieldNo = 0; fieldNo < fieldNames.Count; fieldNo++)
                        fields[fieldNo].Add((csv.GetField(fieldNo)).Trim());                    
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

        public static void ExportToCsv(string fName, DataTable dataTable)
        {
            var fStream = new FileStream(fName, FileMode.Create);
            var textWriter = new StreamWriter(fStream);
            var csv = new CsvWriter(textWriter);

            //write headers
            foreach (DataColumn header in dataTable.Columns)
                csv.WriteField(header.ColumnName);

            csv.NextRecord();

            foreach(DataRow row in dataTable.Rows)
            {
                for (var index = 0; index < dataTable.Columns.Count; index++)
                    csv.WriteField(row[index]);

                csv.NextRecord();
            }

            textWriter.Close();
            fStream.Close();
        }        
    }
}



