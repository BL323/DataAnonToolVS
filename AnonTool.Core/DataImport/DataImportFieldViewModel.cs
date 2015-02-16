using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.DataImport
{
    public class DataImportFieldViewModel : UpdateBase
    {
        private string _header;
        private string _selectedDataType = "string";
        private ObservableCollection<string> _dataTypes = new ObservableCollection<string>() { "string", "int", "decimal", "DateTime" };

        public string Header
        {
            get { return _header; }
            set
            {
                if(_header != value)
                {
                    _header = value;
                    RaisePropertyChanged(() => Header);
                }
            }
        }
        public string SelectedDataType
        {
            get { return _selectedDataType; }
            set
            {
                if (_selectedDataType != value)
                {
                    _selectedDataType = value;
                    RaisePropertyChanged(() => SelectedDataType);
                }
            }
        }
        public ObservableCollection<string> DataTypes
        {
            get { return _dataTypes; }
        }
    }
}
