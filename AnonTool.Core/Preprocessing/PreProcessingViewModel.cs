using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Controllers;
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
    public class PreProcessingViewModel : UpdateBase
    {
        //Private Fields
        private DataTable _inputDataTable;
        private PreprocessingColumnsVm _columnPreprocessorVm;       
        private ICommand _anonymiseCommand;
        private AnonymisationController _anonController = new AnonymisationController();

        //Public Properties
        public DataTable InputDataTable
        {
            get { return _inputDataTable; }
            set
            {
                if(_inputDataTable != value)
                {
                    _inputDataTable = value;
                    UpdateColumnInfo();
                    RaisePropertyChanged(() => InputDataTable);
                }
            }
        }
        public PreprocessingColumnsVm ColumnPreprocessorVm
        {
            get { return _columnPreprocessorVm; }
            set
            {
                if(_columnPreprocessorVm != value)
                {
                    _columnPreprocessorVm = value;
                    RaisePropertyChanged(() => ColumnPreprocessorVm);
                }
            }
        }     

        public ICommand AnonymiseCommand
        {
            get { return _anonymiseCommand ?? (_anonymiseCommand = new RelayCommand(o => InvokeAnonymiseDataSet(), o => true)); }
        }


        //Private Methods
        private void UpdateColumnInfo()
        {
            _columnPreprocessorVm = new PreprocessingColumnsVm(this);
            foreach (DataColumn column in _inputDataTable.Columns)
            {
                var colVm = new PreprocessColumnVm()
                {
                       Header = column.ColumnName,
                       DataType = column.DataType,
                       AttributeType = IdentifierType.NonSensitive,
                       K = 0
                };

                _columnPreprocessorVm.Columns.Add(colVm);
            }

            if (_columnPreprocessorVm.Columns.Count > 0)
                _columnPreprocessorVm.SelectedColumn = _columnPreprocessorVm.Columns.First();

            RaisePropertyChanged(() => ColumnPreprocessorVm);
        }
        private void InvokeAnonymiseDataSet()
        {
            if(ColumnPreprocessorVm != null)
                _anonController.InvokeAnonymisation(InputDataTable, ColumnPreprocessorVm.TranslateToColumnModels());
        }
    }
}
