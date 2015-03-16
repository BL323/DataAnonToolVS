using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnonTool.Core.Hierarchy
{
    public class HierarchyDefintionVm : UpdateBase
    {
        //Private Fields
        private bool _isCustomHierarchySelected;
        private DataTable _hierarchyStringRedactionDefintions = new DataTable();
        private DataTable _hierarchyCustomDefintions = new DataTable();
        private Dictionary<string, LinkedList<string>> _hierarchyStringRedaction;
        private Dictionary<string, LinkedList<string>> _hierarchyCustomDefintion;
        private HierarchyDefinitionOptionsVm _hierarchyDefintionOptionsVm = new HierarchyDefinitionOptionsVm();
        private ICommand _removeLastLevelCommand;
        private ICommand _appendLastLevelCommand;

        // Public Properties
        public bool IsCustomHierarchySelected
        {
            get { return _isCustomHierarchySelected; }
            set
            {
                if(_isCustomHierarchySelected != value)
                {
                    _isCustomHierarchySelected = value;
                    RaisePropertyChanged(() => IsCustomHierarchySelected);
                }
            }
        }
        public DataTable HierarchyStringRedactionDefintions
        {
            get { return _hierarchyStringRedactionDefintions; }
            set
            {
                if(_hierarchyStringRedactionDefintions != value)
                {
                    _hierarchyStringRedactionDefintions = value;
                    RaisePropertyChanged(() => HierarchyStringRedactionDefintions);
                }
            }
        }
        public DataTable HierarchyCustomDefintions
        {
            get { return _hierarchyCustomDefintions; }
            set
            {
                if(_hierarchyCustomDefintions != value)
                {
                    _hierarchyCustomDefintions = value;
                    RaisePropertyChanged(() => HierarchyCustomDefintions);
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
        public ICommand RemoveLastLevelCommand
        {
            get { return _removeLastLevelCommand ?? (_removeLastLevelCommand = new RelayCommand(o => RemoveLastLevel(), o => true)); }
        }
        public ICommand AppendLastLevelCommand
        {
            get { return _appendLastLevelCommand ?? (_appendLastLevelCommand = new RelayCommand(o => AppendLastLevel(), o => true)); }
        }

        private void AppendLastLevel()
        {
            var tempClone = HierarchyCustomDefintions.Clone();
            
            var colCount = tempClone.Columns.Count;
            var columnName = string.Format("Level{0}", colCount);
            var dataCol = new DataColumn()
            {
                ColumnName = columnName,
                DataType = typeof(string),
                DefaultValue = "*"
            };

            tempClone.Columns.Add(dataCol);
  
            //repopulate data
            foreach (DataRow row in HierarchyCustomDefintions.Rows)
                tempClone.ImportRow(row);
                
            HierarchyCustomDefintions = tempClone;
        }
        private void RemoveLastLevel()
        {
            var columnCount = HierarchyCustomDefintions.Columns.Count;

            if (columnCount == 1)
                return;

            var tempClone = HierarchyCustomDefintions.Clone();
            tempClone.Columns.RemoveAt(columnCount -1);

            //repopulate data
            foreach (DataRow row in HierarchyCustomDefintions.Rows)
                tempClone.ImportRow(row);

            HierarchyCustomDefintions = tempClone;
        }

        private void InitCustomHierarchy()
        {
            var lvl0 = "Level0";
            HierarchyCustomDefintions.Columns.Add(lvl0, typeof(string));   
            foreach(var entry in _hierarchyDefintionOptionsVm.UniqueValues)
            {
                var row = HierarchyCustomDefintions.NewRow();
                row.BeginEdit();
                row[lvl0] = entry;
                row.EndEdit();
                HierarchyCustomDefintions.Rows.Add(row);
                row.AcceptChanges();

            }

            HierarchyCustomDefintions.AcceptChanges();
        }
        public void GenerateHierarchy()
        {
            InitCustomHierarchy();
            _hierarchyStringRedaction = StringRedactionHierarchyGenerator.Generate(_hierarchyDefintionOptionsVm.UniqueValues);
            var result = StringRedactionHierarchyGenerator.GenerateH(_hierarchyDefintionOptionsVm.UniqueValues);

            FormatHierarchyStringRedctionForDisplay();
        }
        private void FormatHierarchyStringRedctionForDisplay()
        {
            _hierarchyStringRedactionDefintions.TableName = "Hierarchy Defintions";
            var firstEntry = _hierarchyStringRedaction.First();
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

                _hierarchyStringRedactionDefintions.Columns.Add(dataColumn);
            }

         
            //add rows
            foreach(var item in _hierarchyStringRedaction)
            {
                linkedList = item.Value;
                var row = _hierarchyStringRedactionDefintions.NewRow();
                var nodeVal = linkedList.First;
                for(var level = 0; level < levels; level++)
                {
                    var colRef = string.Format("Level{0}", level);
                    row[colRef] = nodeVal.Value;
                    nodeVal = nodeVal.Next;
                }
                _hierarchyStringRedactionDefintions.Rows.Add(row);
            }

            _hierarchyStringRedactionDefintions.AcceptChanges();

        }
       
        
        public ColumnHierarchy ExtractHierarchy()
        {
            //Selects hierarchy to use
            if(IsCustomHierarchySelected)
            {
                _hierarchyCustomDefintion = CollateCustomHierarchy();

                var customHierarchy = new ColumnHierarchy()
                {
                    AnonymistionValues = _hierarchyCustomDefintion
                };

                return customHierarchy;
            }
            //else string redaction hierarchy
            if (_hierarchyStringRedaction == null)
                return null;

            var columnHierarchy = new ColumnHierarchy()
            {
                AnonymistionValues = _hierarchyStringRedaction
            };

            return columnHierarchy;

        }

        private Dictionary<string, LinkedList<string>> CollateCustomHierarchy()
        {
            var result = new Dictionary<string, LinkedList<string>>();

            if (_hierarchyCustomDefintions == null)
                return null;

            var anonLevels = _hierarchyCustomDefintions.Columns.Count;
            foreach(DataRow row in HierarchyCustomDefintions.Rows)
            {
                var initVal = row["Level0"].ToString();
                var linkList = new LinkedList<string>();
                var node = linkList.AddFirst(initVal);


                for(int index = 1; index <anonLevels; index++)
                {
                    var colName = string.Format("Level{0}", index);
                    var nextVal = row[colName].ToString();
                    node = linkList.AddAfter(node, nextVal);
                }

                result.Add(initVal, linkList);
            }

            return result;
        }
    }
}
