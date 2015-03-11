using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KAnonymisation.Core.Output.PostProcessing
{
    public class QueryViewModel : UpdateBase
    {
        private int _queryNumber;
        private string _selectedDataTable;            
        private ObservableCollection<string> _availableAttributes = new ObservableCollection<string>();
        private ObservableCollection<QueryStatementViewModel> _queryStatments = new ObservableCollection<QueryStatementViewModel>();
        private ICommand _addQueryStatementCommand;
        private ICommand _removeQueryStatementCommand;


        public int QueryNumber
        {
            get { return _queryNumber; }
            set
            {
                if(_queryNumber != value)
                {
                    _queryNumber = value;
                    RaisePropertyChanged(() => QueryNumber);
                    RaisePropertyChanged(() => QueryNumberTitle);
                }
            }
        }
        public string QueryNumberTitle
        {
            get { return string.Format("Query No: #{0}", _queryNumber); }
        }
        public string SelectedDataTable
        {
            get { return _selectedDataTable; }
            set
            {
                if(_selectedDataTable != value)
                {
                    _selectedDataTable = value;
                    RaisePropertyChanged(() => SelectedDataTable);
                }
            }
        }
        public ObservableCollection<string> AvailableAttributes
        {
            get { return _availableAttributes; }
            set
            {
                if(_availableAttributes != value)
                {
                    _availableAttributes = value;
                    RaisePropertyChanged(() => AvailableAttributes);
                }
            }
        }
        public ObservableCollection<string> AvailableDataTables
        {
            get{return new ObservableCollection<string>() {"Input Table", "OutputTable"};}
        }
        public ObservableCollection<QueryStatementViewModel> QueryStatements
        {
            get { return _queryStatments; }
            set
            {
                if(_queryStatments != value)
                {
                    _queryStatments = value;
                    RaisePropertyChanged(() => QueryStatements);
                }
            }
        }
        public ICommand AddQueryStatementCommand
        {
            get { return _addQueryStatementCommand ?? (_addQueryStatementCommand = new RelayCommand(o => AddQueryStatement(), o => true)); }
        }
        public ICommand RemoveQueryStatementCommand
        {
            get { return _removeQueryStatementCommand ?? (_removeQueryStatementCommand = new RelayCommand(o => RemoveQueryStatement(), o => true)); }
        }

        private void AddQueryStatement()
        {
            var queryStatement = new QueryStatementViewModel();
            queryStatement.AvailableAttributes = AvailableAttributes;
            if (AvailableAttributes != null && AvailableAttributes.Count > 0)
                queryStatement.Attribute = AvailableAttributes.First();

            QueryStatements.Add(queryStatement);
        }
        private void RemoveQueryStatement()
        {
            if(QueryStatements.Count > 0)
                QueryStatements.Remove(QueryStatements.Last());
        }

    }
}
