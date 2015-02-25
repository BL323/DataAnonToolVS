using AnonTool.MVVM.Updates;
using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Preprocessing
{
    public class PreprocessingColumnsVm : UpdateBase
    {
        private PreprocessColumnVm _selectedColumn;
        private ObservableCollection<IdentifierType> _availableAttributeTypes = new ObservableCollection<IdentifierType>() 
            { IdentifierType.NonSensitive, IdentifierType.Sensitive, IdentifierType.Quasi, IdentifierType.Explicit };

        private ObservableCollection<PreprocessColumnVm> _columns = new ObservableCollection<PreprocessColumnVm>();

        public PreprocessColumnVm SelectedColumn
        {
            get { return _selectedColumn; }
            set
            {
                if(_selectedColumn != value)
                {
                    _selectedColumn = value;
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
                     K = col.K
                };

                columnModel.Add(colMod);
            }

            return columnModel;
        }
    }
}
