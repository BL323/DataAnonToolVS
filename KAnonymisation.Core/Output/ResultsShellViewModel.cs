using System.Collections.ObjectModel;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Output.PostProcessing;

namespace KAnonymisation.Core.Output
{
    public class ResultsShellViewModel : UpdateBase
    {
        private ResultsViewModel _selectedResult;
        private PostProcessingViewModel _postProcessingVm = new PostProcessingViewModel();
        private ObservableCollection<ResultsViewModel> _results = new ObservableCollection<ResultsViewModel>();

        public int AnonCount { get; set; }
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
        public PostProcessingViewModel PostProcessingVm
        {
            get { return _postProcessingVm; }
            set
            {
                if(_postProcessingVm != value)
                {
                    _postProcessingVm = value;
                    RaisePropertyChanged(() => PostProcessingVm);
                }
            }
        }
        public ObservableCollection<ResultsViewModel> Results
        {
            get { return _results; }
            set
            {
                if(_results != value)
                {
                    _results = value;
                    RaisePropertyChanged(() => Results);
                }
            }
        }
     }
}
