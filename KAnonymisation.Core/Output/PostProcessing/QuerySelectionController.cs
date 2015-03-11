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
    public class QuerySelectionController : UpdateBase
    {
        private int _queryCount;
        private ICommand _addQueryCommand;
        private ICommand _removeQueryCommand;
        private ICommand _randomlyGenerateQueriesCommand;
        private QueryViewModel _selectedQuery;
        private ObservableCollection<string> _availableAttributes;
        private ObservableCollection<QueryViewModel> _queries = new ObservableCollection<QueryViewModel>();

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
            var queryVm = new QueryViewModel() 
            { QueryNumber = ++_queryCount, AvailableAttributes = _availableAttributes };
            
            Queries.Add(queryVm);
        }
        private void RemoveQuery()
        {
            if (SelectedQuery != null)
                Queries.Remove(SelectedQuery);
        }
        private void RandomlyGenerateQueries()
        {
            throw new NotImplementedException();
        }
    }
}
