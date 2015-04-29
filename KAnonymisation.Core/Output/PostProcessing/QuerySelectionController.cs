using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KAnonymisation.Core.Output.PostProcessing
{
    public class QuerySelectionController : UpdateBase
    {
        private int _queryCount;
        private ICommand _addQueryCommand;
        private ICommand _removeQueryCommand;
        private ICommand _randomlyGenerateQueriesCommand;
        private QueryViewModel _selectedQuery;
        private ObservableCollection<string> _availableAttributes;
        private ObservableCollection<QueryViewModel> _queries = new ObservableCollection<QueryViewModel>();

        public DataTable InputTable { get; set; }
        public ICommand AddQueryCommand
        {
            get { return _addQueryCommand ?? (_addQueryCommand = new RelayCommand(o => AddQuery(), o => true)); }
        }
        public ICommand RemoveQueryCommand
        {
            get { return _removeQueryCommand ?? (_removeQueryCommand = new RelayCommand(o => RemoveQuery(), o => true)); }
        }
        public ICommand RandomlyGenerateQueriesCommand
        {
            get { return _randomlyGenerateQueriesCommand ?? (_randomlyGenerateQueriesCommand = new RelayCommand(o => RandomlyGenerateQueries(), o => true)); }
        }
        public QueryViewModel SelectedQuery
        {
            get { return _selectedQuery; }
            set
            {
                if(_selectedQuery != value)
                {
                    _selectedQuery = value;
                    RaisePropertyChanged(() => SelectedQuery);
                }
            }
        }
        public ObservableCollection<QueryViewModel> Queries
        {
            get { return _queries; }
            set
            {
                if(_queries != value)
                {
                    _queries = value;
                    RaisePropertyChanged(() => Queries);
                }
            }
        }


        //Constructor
        public QuerySelectionController(ObservableCollection<string> inAvailableAttributes)
        {
            _availableAttributes = inAvailableAttributes;
        }

        private void AddQuery()
        {
            var queryVm = new QueryViewModel(_availableAttributes) { QueryNumber = ++_queryCount };       
            Queries.Add(queryVm);
        }
        private void RemoveQuery()
        {
            if (Queries != null && Queries.Count > 0)
                Queries.Remove(Queries.Last());
        }
        private void RandomlyGenerateQueries()
        {
            DataRow dataRow = null;
            //Queries.Clear();
            var rand = new Random();
            var queryList = new List<QueryViewModel>();

            var queryVmInput = new QueryViewModel(_availableAttributes) { QueryNumber = ++_queryCount, SelectedDataTable = "Input Table" };
            var queryVmOutput = new QueryViewModel(_availableAttributes){ QueryNumber = ++_queryCount, SelectedDataTable = "OutputTable" };
            queryVmOutput.QueryStatements.Clear();
            queryVmInput.QueryStatements.Clear();

            var attList = new List<string>();
            if (_availableAttributes.Count > 0)
            {
                while (attList.Count < 2)
                {
                    var att = _availableAttributes[rand.Next(0, _availableAttributes.Count - 1)];
                    if (!attList.Contains(att))
                        attList.Add(att);
                }

                foreach (var stat in attList)
                {
                    var queryStatementVmOut = new QueryStatementViewModel();
                    var queryStatementVmIn = new QueryStatementViewModel();

                    var rowData = InputTable.Rows;
                    if(dataRow == null)
                        dataRow = rowData[rand.Next(0, rowData.Count - 1)];

                    var data = dataRow[stat].ToString();

                    queryStatementVmOut.AvailableAttributes = _availableAttributes;
                    queryStatementVmOut.Attribute = stat;
                    queryStatementVmOut.Criteria = data;
                    queryVmOutput.QueryStatements.Add(queryStatementVmOut);

                    queryStatementVmIn.AvailableAttributes = _availableAttributes;
                    queryStatementVmIn.Attribute = stat;
                    queryStatementVmIn.Criteria = data;
                    queryVmInput.QueryStatements.Add(queryStatementVmIn);
                }

            }

            Queries.Add(queryVmInput);
            Queries.Add(queryVmOutput);
        }

    }
}
