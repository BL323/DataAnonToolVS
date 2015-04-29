using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using KAnonymisation.Core.Output;
using System.Data;
using KAnonymisation.Core.Output.PostProcessing;
using System.Collections.ObjectModel;
using KAnonymisation.Core.Hierarchy;
using AnonTool.Core.Hierarchy;
using KAnonymisation.Core.Output.PostProcessing.DataBasedEvaluation;
using System.Collections.Generic;

namespace DataAnonTool.Tests
{
    [TestClass]
    public class PostProcessingTests
    {
        private string _colName = "Test Header";
        private ResultsViewModel _resultsVm;
        private PostProcessingViewModel _postProcessingVm;

        [TestInitialize]
        public void TestSetup()
        {
            _postProcessingVm = new PostProcessingViewModel();
            _resultsVm = new ResultsViewModel() { QueryController = new QuerySelectionController(new ObservableCollection<string>() { _colName }) };
            _postProcessingVm.SelectedResult = _resultsVm;
        }

        [TestMethod]
        public void WildCardMatchTest()
        {
            var inputDt = new DataTable("inputTable");
            var testColumn0 = new DataColumn() { ColumnName = _colName, DataType = typeof(string) };
            inputDt.Columns.Add(testColumn0);

            //Populate DataTables
            var row = inputDt.NewRow();
            row[_colName] = "MK7 7SU";
            inputDt.Rows.Add(row);
            row = inputDt.NewRow();
            row[_colName] = "MK7 8HY";
            inputDt.Rows.Add(row);
            row = inputDt.NewRow();
            row[_colName] = "MK7 7SU";
            inputDt.Rows.Add(row);
            row = inputDt.NewRow();
            row[_colName] = "MK7 7SU";
            inputDt.Rows.Add(row);

            _resultsVm.InputDataTable = inputDt;
            var query0 = new QueryViewModel(new ObservableCollection<string>() { _colName });
            var query1 = new QueryViewModel(new ObservableCollection<string>() { _colName });
            query0.SelectedDataTable = "Input Table";
            query1.SelectedDataTable = "Input Table";

            var queryStatement = new QueryStatementViewModel();
            query0.QueryStatements.Add(queryStatement);
            query1.QueryStatements.Add(queryStatement);
            _resultsVm.QueryController.Queries.Add(query0);
            _resultsVm.QueryController.Queries.Add(query1);

            _resultsVm.QueryController.Queries[0].QueryStatements[0].Criteria = "MK7";
            _resultsVm.QueryController.Queries[0].QueryStatements[0].Attribute = _colName;

            // use % as a wild card to match anything that after MK7
            _resultsVm.QueryController.Queries[1].QueryStatements[0].Criteria = "MK7%";
            _resultsVm.QueryController.Queries[1].QueryStatements[0].Attribute = _colName;

            
            _postProcessingVm.ExtractMetricsCommand.Execute(null);
            var result = _resultsVm.ExtractedMetrics;
            Assert.IsNotNull(result);

            var resultLines = result.Split('\n');
            Assert.IsTrue(resultLines[1].Equals("Query No: #0: 0"));
            Assert.IsTrue(resultLines[2].Equals("Query No: #0: 4"));
        }

        [TestMethod]
        public void MatchOnAnonymisedValuesTest()
        {
            var inputDt = new DataTable("inputTable");
            var outputDt = new DataTable("OutputTable");

            var testColumn0 = new DataColumn() { ColumnName = _colName, DataType = typeof(string) };
            var testColumn1 = new DataColumn() { ColumnName = _colName, DataType = typeof(string) };
            inputDt.Columns.Add(testColumn0);
            outputDt.Columns.Add(testColumn1);

            //Populate DataTables
            var row = inputDt.NewRow();
            row[_colName] = "MK7 7SU";
            inputDt.Rows.Add(row);
            row = inputDt.NewRow();
            row[_colName] = "MK7 8HY";
            inputDt.Rows.Add(row);
            row = inputDt.NewRow();
            row[_colName] = "MK7 7SU";
            inputDt.Rows.Add(row);
            row = inputDt.NewRow();
            row[_colName] = "MK7 7SU";
            inputDt.Rows.Add(row);

            row = outputDt.NewRow();
            row[_colName] = "MK7 7SU";
            outputDt.Rows.Add(row);
            row = outputDt.NewRow();
            row[_colName] = "MK7****";
            outputDt.Rows.Add(row);
            row = outputDt.NewRow();
            row[_colName] = "MK7 7SU";
            outputDt.Rows.Add(row);
            row = outputDt.NewRow();
            row[_colName] = "MK7 7SU";
            outputDt.Rows.Add(row);

            _resultsVm.InputDataTable = inputDt;
            _resultsVm.OutputDataTable = outputDt;

            var query0 = new QueryViewModel(new ObservableCollection<string>() { _colName });
            var query1 = new QueryViewModel(new ObservableCollection<string>() { _colName });
            query0.SelectedDataTable = "Input Table";
            query1.SelectedDataTable = "OutputTable";

            var queryStatement = new QueryStatementViewModel();

            query0.QueryStatements.Add(queryStatement);
            query1.QueryStatements.Add(queryStatement);
            _resultsVm.QueryController.Queries.Add(query0);
            _resultsVm.QueryController.Queries.Add(query1);

            _resultsVm.QueryController.Queries[0].QueryStatements[0].Criteria = "MK7 7SU";
            _resultsVm.QueryController.Queries[0].QueryStatements[0].Attribute = _colName;

            _resultsVm.QueryController.Queries[0].QueryStatements[1].Criteria = "MK7 7SU";
            _resultsVm.QueryController.Queries[0].QueryStatements[1].Attribute = _colName;

            _resultsVm.QueryController.Queries[1].QueryStatements[0].Criteria = "MK7 7SU";
            _resultsVm.QueryController.Queries[1].QueryStatements[0].Attribute = _colName;

            _resultsVm.QueryController.Queries[1].QueryStatements[1].Criteria = "MK7 7SU";
            _resultsVm.QueryController.Queries[1].QueryStatements[1].Attribute = _colName;


            _postProcessingVm.ExtractMetricsCommand.Execute(null);
            var result = _resultsVm.ExtractedMetrics;
            Assert.IsNotNull(result);

            var resultLines = result.Split('\n');
            Assert.IsTrue(resultLines[1].Equals("Query No: #0: 3"));
            Assert.IsTrue(resultLines[2].Equals("Query No: #0: 4"));
        }

        [TestMethod]
        public void ILossCalculationsHierarchyTest()
        {
            var vals = new ObservableCollection<string>() {"foo", "bar", "faa", "foa"};
            var anonHier = StringRedactionHierarchyGenerator.Generate(vals);

            var dict = new Dictionary<string, AnonymisationHierarchy>();
            dict.Add("Test", anonHier);

            _resultsVm.ILossCalcController = new ILossController(dict);

            var iCalc0 = new ILossCalcViewModel(dict);
            iCalc0.SelectedAttribute = iCalc0.Attribues[0];
            iCalc0.SelectedHierarchy = iCalc0.SelectedHierarchy = anonHier;
            iCalc0.SelectedValue = "foo";

            var iCalc1 = new ILossCalcViewModel(dict);
            iCalc1.SelectedAttribute = iCalc1.Attribues[0];
            iCalc1.SelectedHierarchy = iCalc1.SelectedHierarchy = anonHier;
            iCalc1.SelectedValue = "f**";

            var iCalc2 = new ILossCalcViewModel(dict);
            iCalc2.SelectedAttribute = iCalc2.Attribues[0];
            iCalc2.SelectedHierarchy = iCalc2.SelectedHierarchy = anonHier;
            iCalc2.SelectedValue = "***";
            
            
            _resultsVm.ILossCalcController.Calculations.Add(iCalc0);
            _resultsVm.ILossCalcController.Calculations.Add(iCalc1);
            _resultsVm.ILossCalcController.Calculations.Add(iCalc2);
            
            _resultsVm.ILossCalcController.GoCalcCommand.Execute(null);

            // 0 if original leaf value
            Assert.IsTrue(iCalc0.Result.Equals("0.0000"));
            //values above 0 indicate information loss
            Assert.IsTrue(iCalc1.Result.Equals("0.5000"));
            Assert.IsTrue(iCalc2.Result.Equals("0.7500"));

        }



    }
}
