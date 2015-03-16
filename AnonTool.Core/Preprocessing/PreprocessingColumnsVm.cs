using AnonTool.Core.Hierarchy;
using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using AnonTool.UI.Hierarchy;
using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Hierarchy;
using KAnonymisation.SetBased;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnonTool.Core.Preprocessing
{
    public class PreprocessingColumnsVm : UpdateBase
    {
        private PreProcessingViewModel _parentVm;
        private PreprocessColumnVm _selectedColumn;

        private AnonymisationHierarchy _anonymisationHierarchy;

        private ObservableCollection<IdentifierType> _availableAttributeTypes = new ObservableCollection<IdentifierType>() 
            { IdentifierType.NonSensitive, IdentifierType.Sensitive, IdentifierType.Quasi, IdentifierType.Explicit };
        private ObservableCollection<IKAnonymisation> _availableKAnonymisations;
        private ObservableCollection<PreprocessColumnVm> _columns = new ObservableCollection<PreprocessColumnVm>();
        private ICommand _defineHierarchyCommand;
        
        
        public ICommand DefineHierarchyCommand
        {
            get { return _defineHierarchyCommand ?? (_defineHierarchyCommand = new RelayCommand(o => DefineHierarchy(), o => true)); }
        }
        public PreprocessColumnVm SelectedColumn
        {
            get { return _selectedColumn; }
            set
            {
                if(_selectedColumn != value)
                {
                    _selectedColumn = value;
                    if (_selectedColumn.SelectedAnonymisation == null && AvailableKAnonymisations.Count > 0)
                        _selectedColumn.SelectedAnonymisation = AvailableKAnonymisations.First();
                        
                    RaisePropertyChanged(() => SelectedColumn);
                }
            }
        }
        public ObservableCollection<PreprocessColumnVm> Columns
        {
            get { return _columns; }
            set
            {
                if(_columns != value)
                {
                    _columns = value;
                    RaisePropertyChanged(() => Columns);
                }
            }
        }
        public ObservableCollection<IdentifierType> AvailableAttributeTypes
        {
            get { return _availableAttributeTypes; }
        }
        public ObservableCollection<IKAnonymisation> AvailableKAnonymisations
        {
            get { return _availableKAnonymisations; }
            set
            {
                if(_availableKAnonymisations != value)
                {
                    _availableKAnonymisations = value;
                    RaisePropertyChanged(() => AvailableKAnonymisations);
                }
            }
        }

        //Constructor
        public PreprocessingColumnsVm(PreProcessingViewModel parentVm)
        {
            _parentVm = parentVm;
            LoadAvailableKAnonymisations();
        }

        private void LoadAvailableKAnonymisations()
        {
            //To be done dynamically in the final version
            IKAnonymisation defaultSetBasedAnon = new SetBasedAnonymisation();
            IKAnonymisation defaultHierarchyBasedAnon = new HierarchyBasedAnonymisation();

            AvailableKAnonymisations = new ObservableCollection<IKAnonymisation>() { defaultSetBasedAnon, defaultHierarchyBasedAnon};
        }

        private void DefineHierarchy()
        {
            if (SelectedColumn == null)
                return;

            var uniqueVals = GetUniqueValues(_parentVm.InputDataTable, SelectedColumn);

            var hierarchyDefintionShellVm = new HierarchyDefintionShellVm(SelectedColumn.Header, uniqueVals);
            var hierarchyDefintionDialog = new HierarchyDefintionDialog();


            hierarchyDefintionDialog.DataContext = hierarchyDefintionShellVm;
            hierarchyDefintionDialog.ShowDialog();

            SelectedColumn.AnonymisationHierarchy = hierarchyDefintionShellVm.ExtractHierarchy();
        }
        private ObservableCollection<string> GetUniqueValues(System.Data.DataTable dataTable, PreprocessColumnVm SelectedColumn)
        {
            var uniqueVals = new ObservableCollection<string>();
            var vls = new ObservableCollection<string>();

            foreach (DataRow row in dataTable.Rows) 
            { 
                var str = row[SelectedColumn.Header].ToString();

                if (!uniqueVals.Contains(str))
                    uniqueVals.Add(row[SelectedColumn.Header].ToString());
            }
            return uniqueVals;
        }
        public List<ColumnModel> TranslateToColumnModels()
        {
            var columnModel = new List<ColumnModel>();

            foreach(var col in Columns)
            {
                var colMod = new ColumnModel()
                {
                     AttributeType = col.AttributeType,
                     DataType = col.DataType,
                     Header = col.Header,
                     K = col.K,
                     AnonymisationHierarchy = col.AnonymisationHierarchy,
                     KAnonymisation = col.SelectedAnonymisation
                      
                };
                columnModel.Add(colMod);
            }

            return columnModel;
        }
    }
}
