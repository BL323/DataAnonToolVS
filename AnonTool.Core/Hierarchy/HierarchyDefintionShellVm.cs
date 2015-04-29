using System.Collections.ObjectModel;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;

namespace AnonTool.Core.Hierarchy
{
    public class HierarchyDefintionShellVm : UpdateBase
    {
        private HierarchyDefintionVm _hierarchyDefinitionVm = new HierarchyDefintionVm();
        public HierarchyDefintionVm HierarchyDefinitionVm
        {
            get { return _hierarchyDefinitionVm; }
            set
            {
                if(_hierarchyDefinitionVm != value)
                {
                    _hierarchyDefinitionVm = value;
                    RaisePropertyChanged(() => HierarchyDefinitionVm);
                }
            }
        }

        //Constructor
        public HierarchyDefintionShellVm(string columnName, ObservableCollection<string> uniqueVals)
        {
            HierarchyDefinitionVm.HierarchyDefintionOptionsVm.ColumnName = columnName;
            HierarchyDefinitionVm.HierarchyDefintionOptionsVm.UniqueValues = uniqueVals;
            HierarchyDefinitionVm.GenerateHierarchy();
        }

        internal AnonymisationHierarchy ExtractHierarchy()
        {
            return HierarchyDefinitionVm.ExtractHierarchy();
        }
    }
}
