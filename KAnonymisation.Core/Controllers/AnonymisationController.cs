﻿using System;
using System.Collections.Generic;
using System.Data;
using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.Output;
using KAnonymisation.UI.Output;

namespace KAnonymisation.Core.Controllers
{
    public class AnonymisationController
    {
        private ResultsShellViewModel _resultShellVm = new ResultsShellViewModel();
        private ResultsShellView _resultsShellView = new ResultsShellView();

        //Constructor
        public AnonymisationController()
        {
            _resultsShellView.DataContext = _resultShellVm;
        }

        public DataTable InvokeAnonymisation(DataTable dataTable, List<ColumnModel> columnModels, bool displayResults)
        {
            DataTable result = null;
            DataTable inputTable = CloneInputTable(dataTable);

            ConvertTableDataToStrings(ref dataTable);
            //anonymiseDataTable
            result = AnonymiseDataTable(dataTable, columnModels);

            if(displayResults)
                DisplayResult(result, inputTable, columnModels);

            return result;
        }

        private DataTable CloneInputTable(DataTable dataTable)
        {
            var result = dataTable.Clone();
            foreach (DataRow row in dataTable.Rows)
                result.ImportRow(row);

            return result;
        }
        private void ConvertTableDataToStrings(ref DataTable dataTable)
        {
            //table is converted all to strings and then parsed back to original data types later
            DataTable dtClone = dataTable.Clone();
            foreach (DataColumn column in dataTable.Columns)
                dtClone.Columns[column.ColumnName].DataType = typeof(string);
            foreach (DataRow row in dataTable.Rows)
                dtClone.ImportRow(row);
            dataTable = dtClone;
        }
        private void DisplayResult(DataTable dataTable, DataTable inputDataTable, List<ColumnModel> columnModels)
        {
            //Display Data
            var resultVm = new ResultsViewModel();
            resultVm.ColumnModels = columnModels;
            resultVm.OutputDataTable = dataTable;
            resultVm.InputDataTable = inputDataTable;
            resultVm.AnonTitle = String.Format("Anon: {0}", ++_resultShellVm.AnonCount);

            _resultShellVm.Results.Add(resultVm);
            //update selected anonymisations
            _resultShellVm.SelectedResult = resultVm;
            _resultShellVm.PostProcessingVm.SelectedResult = resultVm;

            var resultsShellView = new ResultsShellView() { DataContext = _resultShellVm };
            resultsShellView.ShowDialog();
        }
        private DataTable AnonymiseDataTable(DataTable dataTable, List<ColumnModel> columnModels)
        {
            foreach (var col in columnModels)
                AnonymiseColumn(ref dataTable, col);

            return dataTable;
        }  
        
        //Apply Anon to each column induvidually
        private void AnonymiseColumn(ref DataTable dataTable, ColumnModel columnModel)
        {
            //Invokes KAnonymisation
            if(columnModel != null && columnModel.KAnonymisation != null)
                columnModel.KAnonymisation.ApplyAnonymisation(ref dataTable, columnModel);
        }
        public void DisplayResultsDialog()
        {
            var resultsShellview = new ResultsShellView() {DataContext = _resultShellVm};
            resultsShellview.ShowDialog();
        }
    }
}
