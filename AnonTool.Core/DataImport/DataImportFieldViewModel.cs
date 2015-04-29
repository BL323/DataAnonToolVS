using System.Collections.Generic;
using System.Collections.ObjectModel;
using AnonTool.MVVM.Updates;

namespace AnonTool.Core.DataImport
{
    public class DataImportFieldViewModel : UpdateBase
    {
        private string _header;
        private string _selectedDataType = "string";
        private List<string> _data = new List<string>();
        // data types supported by the tool
        private ObservableCollection<string> _dataTypes = new ObservableCollection<string>() { "string", "int", "double", "date"};

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
        public List<string> Data
        {
            get { return _data; }
            set
            {
                if(_data != value)
                {
                    _data = value;
                    RaisePropertyChanged(() => Data);
                }
            }
        }
        public ObservableCollection<string> DataTypes
        {
            get { return _dataTypes; }
        }
    }
}
