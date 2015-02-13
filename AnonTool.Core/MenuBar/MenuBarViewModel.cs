using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnonTool.Core.MenuBar
{
    public class MenuBarViewModel : UpdateBase
    {
        //Private Fields 
        private ICommand _importDataCommand;
        private ICommand _exportDataCommand;

        //Public Properties
        public ICommand ImportDataCommand
        {
            get { return _importDataCommand ?? (_importDataCommand = new RelayCommand(o => ImportData(), o => true)); }
        }
        public ICommand ExportDataCommand
        {
            get { return _exportDataCommand ?? (_exportDataCommand = new RelayCommand(o => ExportData(), o => true)); }
        }


        private void ImportData()
        {
            throw new NotImplementedException();
        }
        private void ExportData()
        {
            throw new NotImplementedException();
        }

    }
}
