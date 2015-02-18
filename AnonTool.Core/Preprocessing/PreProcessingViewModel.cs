using AnonTool.Infrastructure.IdentifierTypes;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Preprocessing
{
    public class PreProcessingViewModel : UpdateBase
    {
        private DataTable _inputDataTable;
        private PreprocessingColumnsVm _columnPreprocessorVm;

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

        private void UpdateColumnInfo()
        {
            _columnPreprocessorVm = new PreprocessingColumnsVm();
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
    }
}
