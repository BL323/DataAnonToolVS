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
        private ICommand _extractMetricsCommand;

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
        public ICommand ExtractMetricsCommand
        {
            get { return _extractMetricsCommand ?? (_extractMetricsCommand = new RelayCommand(o => ExtractMetrics(), o => true)); }
        }

        private void ExtractMetrics()
        {
            if(SelectedResult != null)
                SelectedResult.ExtractAnonymisationMetrics();
        }
    }
}
