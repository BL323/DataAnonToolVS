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
using System.Data;

namespace AnonTool.Core.MenuBar
{

    public delegate void ImportedData(object sender, DataTable importedDataTable);
    
    public class MenuBarViewModel : UpdateBase
    {
        //Events
        public event ImportedData importedData;
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

                var dataTable = CreateDataTable(dataMapper);
                importedData(this, dataTable);
            }
        }
        private void ExportData()
        {
            throw new NotImplementedException();
        }

        private DataTable CreateDataTable(DataMapper dataMapper)
        {
            var disvm = new DataImportShellViewModel(dataMapper);
            var dataImportDialog = new DataImportDialog();
            dataImportDialog.DataContext = disvm;

            dataImportDialog.ShowDialog();

            if (dataImportDialog.DialogResult != true)
                return null;
            
            var dataTable = disvm.DataImportVm.GenerateDataTable();
            disvm.DataImportVm.PopulateDataTable(ref dataTable);
            return dataTable;
     
        }
    }
}
