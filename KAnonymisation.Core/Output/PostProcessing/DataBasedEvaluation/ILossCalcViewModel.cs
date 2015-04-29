using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;

namespace KAnonymisation.Core.Output.PostProcessing.DataBasedEvaluation
{
    public class ILossCalcViewModel : UpdateBase
    {
        private int _calcNumber;
        private string _result = "#";
        private string _selectedAttribute;
        private string _selectedValue;
        private Dictionary<string, AnonymisationHierarchy> _anonDict;
        private ObservableCollection<string> _attributes = new ObservableCollection<string>();
        private ObservableCollection<string> _values = new ObservableCollection<string>();
        
        public int CalcNumber
        {
            get { return _calcNumber; }
            set
            {
                if(_calcNumber != value)
                {
                    _calcNumber = value;
                    RaisePropertyChanged(() => CalcNumber);
                }
            }
        }
        public string Result
        {
            get { return _result; }
            set
            {
                if(_result != value)
                {
                    _result = value;
                    RaisePropertyChanged(() => Result);
                }
            }
        }
        public string SelectedAttribute
        {
            get { return _selectedAttribute; }
            set
            {
                if (_selectedAttribute != value)
                {
                    _selectedAttribute = value;
                    UpdateValues(_selectedAttribute);
                    UpdateSelectedHierarchy();
                    RaisePropertyChanged(() => SelectedAttribute);
                }
            }
        }
        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if(_selectedValue != value)
                {
                    _selectedValue = value;

                    RaisePropertyChanged(() => SelectedValue);
                }
            }
        }

        private void UpdateSelectedHierarchy()
        {
            if (_anonDict == null)
                return;

            if (_anonDict.ContainsKey(_selectedAttribute))
                SelectedHierarchy = _anonDict[_selectedAttribute];
            else
                SelectedHierarchy = null;
        }
        public ObservableCollection<string> Attribues
        {
            get { return _attributes; }
            set
            {
                if(_attributes != value)
                {
                    _attributes = value;
                    RaisePropertyChanged(() => Attribues);
                }
            }
        }
        public ObservableCollection<string> Values
        {
            get { return _values; }
            set
            {
                if(_values != value)
                {
                    _values = value;
                    RaisePropertyChanged(() => Values);
                }
            }
        }
        public AnonymisationHierarchy SelectedHierarchy { get; set; }
    
        //Constructors
        public ILossCalcViewModel(Dictionary<string, AnonymisationHierarchy> anonDict)
        {
            _anonDict = anonDict;

            foreach(var key in _anonDict.Keys)
            {
                var res = _anonDict[key];
                if (res != null)
                    Attribues.Add(key);
            }

            if (Attribues.Count > 0)
                SelectedAttribute = Attribues.First();

        }

        private void UpdateValues(string selectedAttribute)
        {
            if (_anonDict == null || !_anonDict.ContainsKey(selectedAttribute)
                || _anonDict[selectedAttribute] == null)
                return;

            var hierarchy = _anonDict[selectedAttribute];
            var nodes = hierarchy.GetAllNodes();
            Values.Clear();
            foreach(var node in nodes)
                Values.Add(node.Value);

            //sort for order
            var tempList = Values.ToList();
            tempList.Sort();
            Values = new ObservableCollection<string>(tempList);

            if (Values.Count > 0)
                SelectedValue = Values.First();

        }
    
    }
}
