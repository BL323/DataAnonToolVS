using AnonTool.Infrastructure.DataLoading;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.DataImport
{
    public class DataImportViewModel : UpdateBase
    {
        private Dictionary<string, Type> _dataTypeMapperDict;
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
            InitDataTypeMapperDict();

            if (dataMapper == null)
                return;

            for(var index = 0; index < dataMapper.Headers.Count; index++)
            {
                var dataField = new DataImportFieldViewModel();
                dataField.Header = dataMapper.Headers[index];
                dataField.Data = dataMapper.Fields[index];
                _dataFields.Add(dataField);
            }

        }

        private void InitDataTypeMapperDict()
        {
            _dataTypeMapperDict = new Dictionary<string, Type>();
            _dataTypeMapperDict.Add("string", typeof(string));
            _dataTypeMapperDict.Add("int", typeof(int));
            _dataTypeMapperDict.Add("double", typeof(double));
            //_dataTypeMapperDict.Add("DateTime", typeof(string));
        }
        public DataTable GenerateDataTable()
        {
            try
            {
                var dataTable = new DataTable("Data Table");
                DataColumn column;

                //dynamically build data table columns
                for (var index = 0; index < DataFields.Count; index++)
                {
                    var sysType = _dataTypeMapperDict[DataFields[index].SelectedDataType];
                    column = new DataColumn(DataFields[index].Header, sysType);
                    dataTable.Columns.Add(column);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating DataTable: " + ex.InnerException.Message);
            }
        }
        public void PopulateDataTable(ref DataTable dataTable)
        {
            //dynamically populate the table
            try
            {
                for (var rowIndex = 0; rowIndex < DataFields[0].Data.Count; rowIndex++)
                {
                    var row = dataTable.NewRow();

                    for (var colIndex = 0; colIndex < DataFields.Count; colIndex++)
                    {
                        var colName = DataFields[colIndex].Header;
                        var data = DataFields[colIndex].Data[rowIndex];
                        row[colName] = data;
                    }
                    dataTable.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error populating DataTable: " + ex.InnerException.Message);
            }
        }
    }
}
