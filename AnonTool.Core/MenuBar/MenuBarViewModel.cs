using System;
using System.Data;
using System.Windows.Forms;
using System.Windows.Input;
using AnonTool.Core.DataImport;
using AnonTool.Infrastructure.DataLoading;
using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using AnonTool.UI.DataImport;
using MessageBox = System.Windows.MessageBox;

namespace AnonTool.Core.MenuBar
{

    public delegate void ImportedData(object sender, DataTable importedDataTable);
    
    public class MenuBarViewModel : UpdateBase
    {
        //Events
        public event ImportedData importedData;
       
        //Private Fields 
        private ICommand _importDataCommand;

        //Public Properties
        public ICommand ImportDataCommand
        {
            get { return _importDataCommand ?? (_importDataCommand = new RelayCommand(o => ImportData(), o => true)); }
        }

        private void ImportData()
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter = "csv files (*.csv)| *.csv";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fName = fileDialog.FileName;
                    var dataMapper = DataLoader.LoadCsv(fName);

                    var dataTable = CreateDataTable(dataMapper);
                    importedData(this, dataTable);
                }
            }catch(Exception ex)
            {
                var msgBox = MessageBox.Show(ex.Message, "Error Importing Data");
            }
        }     
        private DataTable CreateDataTable(DataMapper dataMapper)
        {
            var disvm = new DataImportShellViewModel(dataMapper);
            var dataImportDialog = new DataImportDialog();
            dataImportDialog.DataContext = disvm;

            //modal dialog window to map data types to relational attributes
            dataImportDialog.ShowDialog();

            if (dataImportDialog.DialogResult != true)
                return null;
            
            var dataTable = disvm.DataImportVm.GenerateDataTable();
            disvm.DataImportVm.PopulateDataTable(ref dataTable);
            return dataTable;
     
        }
    }
}
