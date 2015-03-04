using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Hierarchy
{
    public class HierarchyDefinitionOptionsVm : UpdateBase
    {
        private string _columnName;
        private ObservableCollection<string> _uniqueValues = new ObservableCollection<string>();

        public string ColumnName { 
            get { return _columnName; }
            set
            {
                if (_columnName != value)
                {
                    _columnName = value;
                    RaisePropertyChanged(() => ColumnName);
                }
            }
        }
        public ObservableCollection<string> UniqueValues
        {
            get { return _uniqueValues; }
            set
            {
                if(_uniqueValues != value)
                {
                    _uniqueValues = value;
                    RaisePropertyChanged(() => UniqueValues);
                }
            }
        }
    }
}
