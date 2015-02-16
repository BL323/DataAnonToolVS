using AnonTool.Infrastructure.DataLoading;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.DataImport
{
    public class DataImportShellViewModel : UpdateBase
    {
        private DataMapper _dataMapper;
        private DataImportViewModel _dataImportVm;

        public DataImportViewModel DataImportVm
        {
            get { return _dataImportVm; }
            set
            {
                if(_dataImportVm != value)
                {
                    _dataImportVm = value;
                    RaisePropertyChanged(() => DataImportVm);
                }
            }
        }

        public DataImportShellViewModel(DataMapper dataMapper)
        {
            _dataMapper = dataMapper;
            _dataImportVm = new DataImportViewModel(_dataMapper);
        }
    }
}
