using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Output.PostProcessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Output
{
    public class ResultsViewModel : UpdateBase
    {
        private bool _outputTableInit = false;
        private string _anonTitle;
        private string _extractedMetrics;
        private DataTable _inputDataTable;
        private DataTable _outputDataTable;
        private QuerySelectionController _queryController;

        public string AnonTitle
        {
            get { return _anonTitle; }
            set
            {
                if(_anonTitle != value)
                {
                    _anonTitle = value;
                    RaisePropertyChanged(() => AnonTitle);
                }
            }
        }
        public string ExtractedMetrics
        {
            get { return _extractedMetrics; }
            set
            {
                if(_extractedMetrics != value)
                {
                    _extractedMetrics = value;
                    RaisePropertyChanged(() => ExtractedMetrics);
                }
            }
        }
        //input table required to calc meterics
        public DataTable InputDataTable
        {
            get { return _inputDataTable; }
            set
            {
                if(_inputDataTable != value)
                {
                    _inputDataTable = value;
                    RaisePropertyChanged(() => InputDataTable);
                }
            }
        }
        public DataTable OutputDataTable
        {
            get { return _outputDataTable; }
            set
            {
                if (_outputDataTable != value)
                {
                    _outputDataTable = value;
                    RaisePropertyChanged(() => OutputDataTable);
                    if(!_outputTableInit)
                    {
                        SetupAttributes();
                        _outputTableInit = true;
                    }
                }
            }
        }
        public QuerySelectionController QueryController
        {
            get { return _queryController; }
            set
            {
                if(_queryController != value)
                {
                    _queryController = value;
                    RaisePropertyChanged(() => QueryController);
                }
            }
        }

        private void SetupAttributes()
        {
            var attributes = new ObservableCollection<string>();

            foreach (DataColumn col in _outputDataTable.Columns)
                attributes.Add(col.ColumnName);

            _queryController = new QuerySelectionController(attributes);
        }

        //Extract Meterics
        public void ExtractAnonymisationMetrics() 
        {
            var result = "Extracted Metrics";
            
            if (QueryController == null)
                return;

            foreach (var query in QueryController.Queries)
                ExtractQueryMeteric(ref result, query);
            
            
            ExtractedMetrics = result;
        }

        private void ExtractQueryMeteric(ref string result, QueryViewModel query)
        {
            var occuranceCount = 0;
            var kvp = new Dictionary<string, string>();

            

            var dataTable = (query.SelectedDataTable == "Input DataTable") ? _inputDataTable : _outputDataTable;
            
            //set out criteria for rows
            foreach(DataRow row in dataTable.Rows)
            {
                //var attribute0Satisfied = (row[query.SelectedAttribute0].ToString() == query.SelectedCriteria0) ? true : false;
                //var attribute1Satisfied = (row[query.SelectedAttribute1].ToString() == query.SelectedCriteria1) ? true : false;
                //var attribute2Satisfied = (row[query.SelectedAttribute2].ToString() == query.SelectedCriteria2) ? true : false;

                //handle data
                //CriteriaSatisfied

               // if (attribute0Satisfied && attribute1Satisfied && attribute2Satisfied)
               //     occuranceCount++;

            }



            result = string.Format("{0}\n{1}: {2}", result, query.QueryNumberTitle, occuranceCount);
        }
    }
}
