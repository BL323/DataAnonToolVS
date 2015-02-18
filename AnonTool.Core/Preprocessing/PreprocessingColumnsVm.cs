using AnonTool.Infrastructure.IdentifierTypes;
using AnonTool.MVVM.Updates;
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
    }
}
