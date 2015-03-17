using AnonTool.MVVM.Updates;
using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.Output.PostProcessing;
using KAnonymisation.Core.Output.PostProcessing.DataBasedEvaluation;
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
        private List<ColumnModel> _columnModels;
        private QuerySelectionController _queryController;
        private ILossController _iLossCalcController;

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
        public List<ColumnModel> ColumnModels
        {
            get { return _columnModels; }
            set
            {
                if(_columnModels != value)
                {
                    _columnModels = value;
                    RaisePropertyChanged(() => ColumnModels);
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
        public ILossController ILossCalcController
        {
            get { return _iLossCalcController; }
            set
            {
                if(_iLossCalcController != value)
                {
                    _iLossCalcController = value;
                    RaisePropertyChanged(() => ILossCalcController);
                }
            }
        }

        private void SetupAttributes()
        {
            var attributes = new ObservableCollection<string>();

            foreach (DataColumn col in _outputDataTable.Columns)
                attributes.Add(col.ColumnName);

            _queryController = new QuerySelectionController(attributes);

            var anonAttributeDict = GenerateAnonAttributeDict(attributes, ColumnModels);
            _iLossCalcController = new ILossController(anonAttributeDict);
        }
        private Dictionary<string, AnonymisationHierarchy> GenerateAnonAttributeDict(ObservableCollection<string> attributes, List<ColumnModel> columnModels)
        {
            var result = new Dictionary<string, AnonymisationHierarchy>();

            foreach(var att in attributes)
            {
                if(!result.ContainsKey(att))
                    result.Add(att, null);
            }

            if (ColumnModels != null)
            {
                foreach (var col in ColumnModels)
                {
                    if (result.ContainsKey(col.Header))
                        result[col.Header] = col.AnonymisationHierarchy;
                } 
            }

            return result;
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
            var dataTable = (query.SelectedDataTable == "Input Table") ? _inputDataTable : _outputDataTable;
            
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
                if (statement.Criteria == null)
                    break;

                //stop unecessary processing
                if (attributesSatisfied == false)
                    return;

                //Output Set
                if (row[statement.Attribute].ToString().StartsWith("{"))
                    SetBasedMatch(ref attributesSatisfied, row, statement);
                else if (statement.Criteria.Contains("%")) 
                    WildCardMatch(ref attributesSatisfied, row, statement);
                else if (row[statement.Attribute].ToString() != statement.Criteria) // extact match
                    attributesSatisfied = false;
            }      
        }
        private void SetBasedMatch(ref bool attributesSatisfied, DataRow row, QueryStatementViewModel statement)
        {
            var str = row[statement.Attribute].ToString().TrimStart('{');
            str = str.TrimEnd('}');
            
            var items = str.Split(',');
            var trimItems = new List<string>();
            foreach (var item in items)
                trimItems.Add(item.Trim());

            foreach(var item in trimItems)
            {
                if (statement.Criteria.Contains("%"))
                {
                    var st = statement.Criteria.Replace("%", "");
                    if (item.StartsWith(st))
                        return;
                }
                else if (statement.Criteria == item) // extact match
                    return;
            }
            
               attributesSatisfied = false;

        }
        private void WildCardMatch(ref bool attributesSatisfied, DataRow row, QueryStatementViewModel statement)
        {
            var str = statement.Criteria.Replace("%","");

            if (!row[statement.Attribute].ToString().StartsWith(str))
                attributesSatisfied = false;
        }
    }
}
