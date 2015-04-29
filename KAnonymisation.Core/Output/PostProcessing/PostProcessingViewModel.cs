using System.Windows.Input;
using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;

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
