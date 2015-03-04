using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Hierarchy
{
    public class HierarchyDefintionVm :UpdateBase
    {
        private DataTable _hierarchyDefintions;
        private HierarchyDefinitionOptionsVm _hierarchyDefintionOptionsVm = new HierarchyDefinitionOptionsVm();

        public DataTable HierarchyDefintions
        {
            get { return _hierarchyDefintions; }
            set
            {
                if(_hierarchyDefintions != value)
                {
                    _hierarchyDefintions = value;
                    RaisePropertyChanged(() => HierarchyDefintions);
                }
            }
        }
        public HierarchyDefinitionOptionsVm HierarchyDefintionOptionsVm
        {
            get { return _hierarchyDefintionOptionsVm; }
            set
            {
                if(_hierarchyDefintionOptionsVm != value)
                {
                    _hierarchyDefintionOptionsVm = value;
                    RaisePropertyChanged(() => HierarchyDefintionOptionsVm);
                }
            }
        }
   

        internal KAnonymisation.Core.Hierarchy.ColumnHierarchy ExtractHierarchy()
        {
            throw new NotImplementedException();
        }
    }
}
