using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Output
{
    public class ResultsViewModel : UpdateBase
    {
        private string _anonTitle;
        private string _extractedMeterics;
        private DataTable _inputDataTable;
        private DataTable _outputDataTable;

        public string AnonTitle
        {
            get { return _anonTitle; }
            set
            {
                if(_anonTitle != value)
                {
                    _anonTitle = value;
                    RaisePropertyChanged(() => AnonTitle);
                }
            }
        }
        public string ExtractedMeterics
        {
            get { return _extractedMeterics; }
            set
            {
                if(_extractedMeterics != value)
                {
                    _extractedMeterics = value;
                    RaisePropertyChanged(() => ExtractedMeterics);
                }
            }
        }
        //input table required to calc meterics
        public DataTable InputDataTable
        {
            get { return _inputDataTable; }
            set
            {
                if(_inputDataTable != value)
                {
                    _inputDataTable = value;
                    RaisePropertyChanged(() => InputDataTable);
                }
            }
        }
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


        //Extract Meterics
        public void ExtractAnonymisationMeterics() 
        {
            ExtractedMeterics = "Extracted\nMeterics";
        }
    }
}
