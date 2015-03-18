using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KAnonymisation.Core.Output.PostProcessing.DataBasedEvaluation
{
    public class ILossController : UpdateBase
    {
        private int _calcCount = 1;
        private Dictionary<string, AnonymisationHierarchy> _attributeAnonDict;
        private ObservableCollection<ILossCalcViewModel> _calculations = new ObservableCollection<ILossCalcViewModel>();
        private ICommand _addCalcCommand;
        private ICommand _removeCalcCommand;
        private ICommand _randomlyGenerateCalcsCommand;
        private ICommand _goCalcCommand;

        public ObservableCollection<ILossCalcViewModel> Calculations
        {
            get { return _calculations; }
            set
            {
                if(_calculations != value)
                {
                    _calculations = value;
                    RaisePropertyChanged(() => Calculations);
                }
            }
        }
        public ICommand AddCalcCommand
        {
            get { return _addCalcCommand ?? (_addCalcCommand = new RelayCommand(o => AddCalc(), o => true)); }
        }
        public ICommand RemoveCalcCommand
        {
            get { return _removeCalcCommand ?? (_removeCalcCommand = new RelayCommand(o => RemoveCalc(), o => true)); }
        }
        public ICommand RandomlyGenerateCalcsCommand
        {
            get { return _randomlyGenerateCalcsCommand ?? (_randomlyGenerateCalcsCommand = new RelayCommand(o => RandomlyGenerate(), o => true)); }
        }
        public ICommand GoCalcCommand
        {
            get{return _goCalcCommand ?? (_goCalcCommand = new RelayCommand(o => GoCalc(), o => true));}
        }

        //Constructor
        public ILossController(Dictionary<string, AnonymisationHierarchy> attributeHierarchyDict)
        {
            _attributeAnonDict = attributeHierarchyDict;
        }

        private void AddCalc()
        {
            var calc = new ILossCalcViewModel(_attributeAnonDict) { CalcNumber = _calcCount++ };
            Calculations.Add(calc);
        }
        private void RemoveCalc()
        {
            if (Calculations != null & Calculations.Count > 0)
                Calculations.Remove(Calculations.Last());
        }
        private void RandomlyGenerate ()
        {
            throw new NotImplementedException();
        }
        private void GoCalc()
        {
            foreach(var calc in Calculations)
            {
                var hierarchy = calc.SelectedHierarchy;
                var node = hierarchy.FindNode(calc.SelectedValue);


                // (Leaf Node Descendents  - 1) / leaf set of attributes A
                double attributeLeaves = hierarchy.LeafNodes(hierarchy.RootNode).Count;
                double leafNodeDescendents = (node.IsLeaf) ? 1 : hierarchy.LeafNodes(node).Count;

                var result = (leafNodeDescendents - 1) / attributeLeaves;
                calc.Result = result.ToString();
            }


        }
    }
}
