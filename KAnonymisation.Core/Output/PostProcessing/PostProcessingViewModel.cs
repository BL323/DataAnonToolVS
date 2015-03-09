using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KAnonymisation.Core.Output.PostProcessing
{
    public class PostProcessingViewModel : UpdateBase
    {
        private ResultsViewModel _selectedResult;
        private ICommand _extractMetericsCommand;

        public ResultsViewModel SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                if(_selectedResult != value)
                {
                    _selectedResult = value;
                    RaisePropertyChanged(() => SelectedResult);
                }
            }
        }
        public ICommand ExtractMetericsCommand
        {
            get { return _extractMetericsCommand ?? (_extractMetericsCommand = new RelayCommand(o => ExtractMeterics(), o => true)); }
        }

        private void ExtractMeterics()
        {
            if(SelectedResult != null)
                SelectedResult.ExtractAnonymisationMeterics();
        }
    }
}
