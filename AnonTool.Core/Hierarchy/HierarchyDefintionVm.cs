using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;
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
        private DataTable _hierarchyDefintions = new DataTable();
        private Dictionary<string, LinkedList<string>> _hierarchy;
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

        public void GenerateHierarchy()
        {
            _hierarchy = StringRedactionHierarchyGenerator.Generate(_hierarchyDefintionOptionsVm.UniqueValues);
            FormatHierarchyForDisplay();
        }
        private void FormatHierarchyForDisplay()
        {
            _hierarchyDefintions.TableName = "Hierarchy Defintions";
            var firstEntry = _hierarchy.First();
            var linkedList = firstEntry.Value;

            var levels = linkedList.Count;
            
            //set columns
            for(var level = 0; level < levels; level++)
            {
                var dataColumn = new DataColumn() 
                {  
                    ColumnName = string.Format("Level{0}", level),
                    DataType = typeof(string)           
                };

                _hierarchyDefintions.Columns.Add(dataColumn);
            }

         
            //add rows
            foreach(var item in _hierarchy)
            {
                linkedList = item.Value;
                var row = _hierarchyDefintions.NewRow();
                var nodeVal = linkedList.First;
                for(var level = 0; level < levels; level++)
                {
                    var colRef = string.Format("Level{0}", level);
                    row[colRef] = nodeVal.Value;
                    nodeVal = nodeVal.Next;
                }
                _hierarchyDefintions.Rows.Add(row);
            }

            _hierarchyDefintions.AcceptChanges();

        }
        public ColumnHierarchy ExtractHierarchy()
        {
            if (_hierarchy == null)
                return null;

            var columnHierarchy = new ColumnHierarchy()
            {
                AnonymistionValues = _hierarchy
            };

            return columnHierarchy;
        }
    }
}
