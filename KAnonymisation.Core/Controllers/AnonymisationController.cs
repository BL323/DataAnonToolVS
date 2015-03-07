﻿using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.Output;
using KAnonymisation.UI.Output;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Controllers
{
    public class AnonymisationController
    {
        public DataTable InvokeAnonymisation(DataTable dataTable, List<ColumnModel> columnModels)
        {
            DataTable result = null;

            ConvertTableDataToStrings(ref dataTable);
            //anonymiseDataTable
            result = AnonymiseDataTable(dataTable, columnModels);
            DisplayResult(result);

            return result;
        }
        private void ConvertTableDataToStrings(ref DataTable dataTable)
        {
            DataTable dtClone = dataTable.Clone();
            foreach (DataColumn column in dataTable.Columns)
                dtClone.Columns[column.ColumnName].DataType = typeof(string);
            foreach (DataRow row in dataTable.Rows)
                dtClone.ImportRow(row);
            dataTable = dtClone;
        }
        private void DisplayResult(DataTable dataTable)
        {
            //Display Data
            var resultVm = new ResultsViewModel();
            resultVm.OutputDataTable = dataTable;
            var resultDialog = new ResultWindowView();
            resultDialog.DataContext = resultVm;
            resultDialog.Show();
        }
        private DataTable AnonymiseDataTable(DataTable dataTable, List<ColumnModel> columnModels)
        {
            foreach (var col in columnModels)
                AnonymiseColumn(ref dataTable, col);

            return dataTable;
        }
     
        
        //Apply Anon to each col induvidually
        private void AnonymiseColumn(ref DataTable dataTable, ColumnModel columnModel)
        {
            //Apply KAnonymisation
            if(columnModel != null && columnModel.KAnonymisation != null)
                columnModel.KAnonymisation.ApplyAnonymisation(ref dataTable, columnModel);
        }
    }
}