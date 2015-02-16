using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;
using AnonTool.Infrastructure.DataLoading;
using AnonTool.Core.DataImport;
using AnonTool.UI.DataImport;

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
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "csv files (*.csv)| *.csv";
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                var fName = fileDialog.FileName;
                
                var dataMapper = DataLoader.LoadCsv(fName);

                UpdateDataTypes(dataMapper);

                //var dataTable = DataLoader.GenerateDataTable(dataMapper);
                //DataLoader.PopulateDataTable(dataMapper, ref dataTable);
            }
        }
        private void ExportData()
        {
            throw new NotImplementedException();
        }
        private void UpdateDataTypes(DataMapper dataMapper)
        {

            var disvm = new DataImportShellViewModel(dataMapper);
            var dataImportDialog = new DataImportDialog();
            dataImportDialog.DataContext = disvm;

            dataImportDialog.ShowDialog();
           
        }
    }
}
