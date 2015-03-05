using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Hierarchy.Show.ViewModels
{
    public class ResultViewModel : UpdateBase
    {
        private DataTable _outputDataTable;
        public DataTable OutputDataTable
        {
            get { return _outputDataTable; }
            set
            {
                if (_outputDataTable != value)
                {
                    _outputDataTable = value;
                    RaisePropertyChanged(() => OutputDataTable);
                }
            }
        }
    }
}
