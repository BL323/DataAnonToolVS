using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
