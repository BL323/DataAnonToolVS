using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnonTool.Core.Preprocessing;
using System.Data;
using AnonTool.Core.Hierarchy;
using System.Collections.ObjectModel;

namespace DataAnonTool.Tests
{
    [TestClass]
    public class AnonymisationTests
    {
        PreProcessingViewModel _preprocessorVm;

        [TestInitialize]
        public void SetupTests()
        {
            _preprocessorVm = new PreProcessingViewModel(false);
            _preprocessorVm.ColumnPreprocessorVm = new PreprocessingColumnsVm(_preprocessorVm, false);
        }

        [TestMethod]
        public void EmptyColumnsTest()
        {         
            var columnModels = _preprocessorVm.ColumnPreprocessorVm.TranslateToColumnModels();

            Assert.IsNotNull(columnModels);
            Assert.IsTrue(columnModels.Count == 0);
        }

        [TestMethod]
        public void DefaultExplicitAnonymisationTest()
        {
            var colName = "Test Header";
            var columnVm = new PreprocessColumnVm();
            columnVm.Header = colName;
            columnVm.DataType = typeof(string);
            //SetBased Anonymisation
            columnVm.SelectedAnonymisation = _preprocessorVm.ColumnPreprocessorVm.AvailableKAnonymisations[0];
            _preprocessorVm.ColumnPreprocessorVm.Columns.Add(columnVm);

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = colName, DataType = typeof(string) });
            var row = dataTable.NewRow();
            row[colName] = "foo";
            dataTable.Rows.Add(row);

            _preprocessorVm.InputDataTable = dataTable;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AttributeType = KAnonymisation.Core.IdentifierTypes.IdentifierType.Explicit;

            _preprocessorVm.AnonymiseCommand.Execute(null);
            var outputDataTable = _preprocessorVm.OutputDataTableTester;
            var result = outputDataTable.Rows[0][colName].ToString();
            Assert.IsTrue(result.Equals("*"));

        }

        [TestMethod]
        public void DefaultSenstiveAnonymisationTest()
        {
            var colName = "Test Header";
            var columnVm = new PreprocessColumnVm();
            columnVm.Header = colName;
            columnVm.DataType = typeof(string);
            //SetBased Anonymisation
            columnVm.SelectedAnonymisation = _preprocessorVm.ColumnPreprocessorVm.AvailableKAnonymisations[0];
            _preprocessorVm.ColumnPreprocessorVm.Columns.Add(columnVm);

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = colName, DataType = typeof(string) });
            var row = dataTable.NewRow();
            row[colName] = "foo";
            dataTable.Rows.Add(row);

            _preprocessorVm.InputDataTable = dataTable;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AttributeType = KAnonymisation.Core.IdentifierTypes.IdentifierType.Sensitive;

            _preprocessorVm.AnonymiseCommand.Execute(null);
            var outputDataTable = _preprocessorVm.OutputDataTableTester;
            var result = outputDataTable.Rows[0][colName].ToString();
            Assert.IsTrue(result.Equals("foo"));
        }

        [TestMethod]
        public void DefaultNonSenstiveAnonymisationTest()
        {
            var colName = "Test Header";
            var columnVm = new PreprocessColumnVm();
            columnVm.Header = colName;
            columnVm.DataType = typeof(string);
            //SetBased Anonymisation
            columnVm.SelectedAnonymisation = _preprocessorVm.ColumnPreprocessorVm.AvailableKAnonymisations[0];
            _preprocessorVm.ColumnPreprocessorVm.Columns.Add(columnVm);

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = colName, DataType = typeof(string) });
            var row = dataTable.NewRow();
            row[colName] = "foo";
            dataTable.Rows.Add(row);

            _preprocessorVm.InputDataTable = dataTable;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AttributeType = KAnonymisation.Core.IdentifierTypes.IdentifierType.NonSensitive;

            _preprocessorVm.AnonymiseCommand.Execute(null);
            var outputDataTable = _preprocessorVm.OutputDataTableTester;
            var result = outputDataTable.Rows[0][colName].ToString();
            Assert.IsTrue(result.Equals("foo"));
        }

        [TestMethod]
        public void DefaultQuasiSetBasedAnonymisationTest()
        {
            var colName = "Test Header";
            var columnVm = new PreprocessColumnVm();
            columnVm.Header = colName;
            //SetBased Anonymisation
            columnVm.SelectedAnonymisation = _preprocessorVm.ColumnPreprocessorVm.AvailableKAnonymisations[0];
            _preprocessorVm.ColumnPreprocessorVm.Columns.Add(columnVm);

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = colName, DataType = typeof(string) });
       
            var row = dataTable.NewRow();
            row[colName] = 1;
            dataTable.Rows.Add(row);
            
            row = dataTable.NewRow();
            row[colName] = 1;
            dataTable.Rows.Add(row);
            
            row = dataTable.NewRow();
            row[colName] = 1;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 99;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 70;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 85;
            dataTable.Rows.Add(row);

            _preprocessorVm.InputDataTable = dataTable;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].K = 3;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AttributeType = KAnonymisation.Core.IdentifierTypes.IdentifierType.Quasi;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].DataType = typeof(int);

            _preprocessorVm.AnonymiseCommand.Execute(null);
            var outputDataTable = _preprocessorVm.OutputDataTableTester;

            var row0 = outputDataTable.Rows[0][colName];
            var row1 = outputDataTable.Rows[1][colName];
            var row2 = outputDataTable.Rows[2][colName];
            Assert.IsTrue(row0.Equals("1"));
            Assert.IsTrue(row1.Equals("1"));
            Assert.IsTrue(row2.Equals("1"));

            var row3 = outputDataTable.Rows[3][colName];
            Assert.IsTrue(row3.ToString().Contains("70") && row3.ToString().Contains("85") && row3.ToString().Contains("99"));
            var row4 = outputDataTable.Rows[4][colName];
            Assert.IsTrue(row4.ToString().Contains("70") && row4.ToString().Contains("85") && row4.ToString().Contains("99"));
            var row5 = outputDataTable.Rows[5][colName];
            Assert.IsTrue(row5.ToString().Contains("70") && row5.ToString().Contains("85") && row5.ToString().Contains("99"));
        }

        [TestMethod]
        public void DefaultQuasiSetBasedAnonymisationNonKMatchingItemsTest()
        {
            var colName = "Test Header";
            var columnVm = new PreprocessColumnVm();
            columnVm.Header = colName;
            //SetBased Anonymisation
            columnVm.SelectedAnonymisation = _preprocessorVm.ColumnPreprocessorVm.AvailableKAnonymisations[0];
            _preprocessorVm.ColumnPreprocessorVm.Columns.Add(columnVm);

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = colName, DataType = typeof(string) });

            var row = dataTable.NewRow();
            row[colName] = 1;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 1;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 99;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 70;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = 85;
            dataTable.Rows.Add(row);

            _preprocessorVm.InputDataTable = dataTable;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].K = 3;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AttributeType = KAnonymisation.Core.IdentifierTypes.IdentifierType.Quasi;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].DataType = typeof(int);

            _preprocessorVm.AnonymiseCommand.Execute(null);
            var outputDataTable = _preprocessorVm.OutputDataTableTester;

            var row0 = outputDataTable.Rows[0][colName];
            Assert.IsTrue(row0.ToString().Contains("1") && row0.ToString().Contains("70") && row0.ToString().Contains("85") && row0.ToString().Contains("99"));
            var row1 = outputDataTable.Rows[1][colName];
            Assert.IsTrue(row1.ToString().Contains("1") && row1.ToString().Contains("70") && row1.ToString().Contains("85") && row1.ToString().Contains("99"));
            var row2 = outputDataTable.Rows[2][colName];
            Assert.IsTrue(row2.ToString().Contains("1") && row2.ToString().Contains("70") && row2.ToString().Contains("85") && row2.ToString().Contains("99"));
            var row3 = outputDataTable.Rows[3][colName];
            Assert.IsTrue(row3.ToString().Contains("1") && row3.ToString().Contains("70") && row3.ToString().Contains("85") && row3.ToString().Contains("99"));
            var row4 = outputDataTable.Rows[4][colName];
            Assert.IsTrue(row4.ToString().Contains("1") && row4.ToString().Contains("70") && row4.ToString().Contains("85") && row4.ToString().Contains("99"));
        }

        [TestMethod]
        public void DefaultQuasiHierarchyAnonymisationTest()
        {
            var colName = "Test Header";
            var columnVm = new PreprocessColumnVm();
            columnVm.Header = colName;
           
            _preprocessorVm.ColumnPreprocessorVm.Columns.Add(columnVm);

            var obsCollection = new ObservableCollection<string>() { "foo", "faa", "fbb" };
            var anonHier = StringRedactionHierarchyGenerator.Generate(obsCollection);

            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = colName, DataType = typeof(string) });

            var row = dataTable.NewRow();
            row[colName] = "foo";
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = "faa";
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[colName] = "fbb";
            dataTable.Rows.Add(row);

            _preprocessorVm.InputDataTable = dataTable;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].K = 3;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AttributeType = KAnonymisation.Core.IdentifierTypes.IdentifierType.Quasi;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].DataType = typeof(string);
            //HierarchyBased Anonymisation
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].AnonymisationHierarchy = anonHier;
            _preprocessorVm.ColumnPreprocessorVm.Columns[0].SelectedAnonymisation = _preprocessorVm.ColumnPreprocessorVm.AvailableKAnonymisations[1];

            _preprocessorVm.AnonymiseCommand.Execute(null);
            var outputDataTable = _preprocessorVm.OutputDataTableTester;

            var row0 = outputDataTable.Rows[0][colName].ToString();
            Assert.IsTrue(row0.Equals("f**"));
            var row1 = outputDataTable.Rows[1][colName].ToString();
            Assert.IsTrue(row0.Equals("f**"));
            var row2 = outputDataTable.Rows[2][colName].ToString();
            Assert.IsTrue(row0.Equals("f**"));
        }
    }
}
