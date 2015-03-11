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
            var dataTable = (query.SelectedDataTable == "Input DataTable") ? _inputDataTable : _outputDataTable;
            
            //set out criteria for rows
            foreach(DataRow row in dataTable.Rows)
            {
                var attributesSatisfied = true;

                EvaluateQueryStatement(ref attributesSatisfied, row, query.QueryStatements);


                if (attributesSatisfied)
                    ++occuranceCount;
            }


            result = string.Format("{0}\n{1}: {2}", result, query.QueryNumberTitle, occuranceCount);
        }

        ///<summary>
        /// {} Denotes Set
        /// [] Denotes Range
        /// % Wild Card
        ///</summary>
        private void EvaluateQueryStatement(ref bool attributesSatisfied, DataRow row, ObservableCollection<QueryStatementViewModel> queryStatements)
        {
            //evaluate each statement induvidually
            foreach(var statement in queryStatements)
            {
                //stop unecessary processing
                if (attributesSatisfied == false)
                    return;


                if (statement.Criteria.Contains("%")) 
                    WildCardMatch(ref attributesSatisfied, row, statement);
                else if (row[statement.Attribute].ToString() != statement.Criteria) // extact match
                    attributesSatisfied = false;
            }      
        }
        private void WildCardMatch(ref bool attributesSatisfied, DataRow row, QueryStatementViewModel statement)
        {
            var str = statement.Criteria.Replace("%","");

            if (!row[statement.Attribute].ToString().StartsWith(str))
                attributesSatisfied = false;
        }
    }
}
