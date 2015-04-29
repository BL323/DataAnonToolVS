using System.Collections.ObjectModel;
using AnonTool.MVVM.Updates;

namespace KAnonymisation.Core.Output.PostProcessing
{
    public class QueryStatementViewModel : UpdateBase
    {
        private string _attribute;
        private string _criteria;
        private ObservableCollection<string> _availableAttributes = new ObservableCollection<string>();

        public string Attribute
        {
            get { return _attribute; }
            set
            {
                if(_attribute != value)
                {
                    _attribute = value;
                    RaisePropertyChanged(() => Attribute);
                }
            }
        }
        public string Criteria
        {
            get { return _criteria; }
            set
            {
                if(_criteria != value)
                {
                    _criteria = value;
                    RaisePropertyChanged(() => Criteria);
                }
            }
        }
        public ObservableCollection<string> AvailableAttributes
        {
            get { return _availableAttributes; }
            set
            {
                if (_availableAttributes != value)
                {
                    _availableAttributes = value;
                    RaisePropertyChanged(() => AvailableAttributes);
                }
            }
        }
    }
}
