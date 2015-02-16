using AnonTool.Infrastructure.DataLoading;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.DataImport
{
    public class DataImportViewModel : UpdateBase
    {
        private ObservableCollection<DataImportFieldViewModel> _dataFields = new ObservableCollection<DataImportFieldViewModel>();
        public ObservableCollection<DataImportFieldViewModel> DataFields
        {
            get { return _dataFields; }
            set
            {
                if (_dataFields != value)
                {
                    _dataFields = value;
                    RaisePropertyChanged(() => DataFields);
                }
            }
        }

        public DataImportViewModel(DataMapper dataMapper)
        {
            if (dataMapper == null)
                return;

            foreach (var header in dataMapper.Headers)
            {
                var dataField = new DataImportFieldViewModel();
                dataField.Header = header;
                _dataFields.Add(dataField);
            }

        }
    }
}
