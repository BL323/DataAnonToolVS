using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string _newNodeValue;
        private DataTable _hierarchyCustomDefintions = new DataTable();
        private AnonymisationHierarchy _hierarchyStrRedaction;
        private AnonymisationHierarchy _hierarchyCustom; 
        private HierarchyDefinitionOptionsVm _hierarchyDefintionOptionsVm = new HierarchyDefinitionOptionsVm();
        private ObservableCollection<Node> _editList = new ObservableCollection<Node>();
        private ICommand _removeLastLevelCommand;
        private ICommand _appendLastLevelCommand;
        private ICommand _addToEditListCommand;
        private ICommand _clearEditListCommand;
        private ICommand _insertNodeCommand;
        private ICommand _removeNodeCommand;

        // Public Properties
        public bool IsCustomHierarchySelected
        {
            get { return _isCustomHierarchySelected; }
            set
            {
                if (_isCustomHierarchySelected != value)
                {
                    _isCustomHierarchySelected = value;
                    RaisePropertyChanged(() => IsCustomHierarchySelected);
                }
            }
        }
        public string NewNodeValue
        {
            get { return _newNodeValue; }
            set
            {
                if(_newNodeValue != value)
                {
                    _newNodeValue = value;
                    RaisePropertyChanged(() => NewNodeValue);
                }
            }
        }
        public AnonymisationHierarchy HierarchyStrRedaction
        {
            get { return _hierarchyStrRedaction; }
            set
            {
                if(_hierarchyStrRedaction != value)
                {
                    _hierarchyStrRedaction = value;
                    RaisePropertyChanged(() => HierarchyStrRedaction);
                }
            }
        }
        public AnonymisationHierarchy HierarchyCustom
        {
            get { return _hierarchyCustom; }
            set
            {
                if(_hierarchyCustom != value)
                {
                    _hierarchyCustom = value;
                    RaisePropertyChanged(() => HierarchyCustom);
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
        public ObservableCollection<Node> EditList
        {
            get { return _editList; }
            set
            {
                if(_editList != value)
                {
                    _editList = value;
                    RaisePropertyChanged(() => EditList);
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
        public ICommand AddToEditListCommand
        {
            get { return _addToEditListCommand ?? (_addToEditListCommand = new RelayCommand(o => AddItemToEditList(), o => true)); }
   
        }
        public ICommand ClearEditListCommand
        {
            get { return _clearEditListCommand ?? (_clearEditListCommand = new RelayCommand(o => ClearEditList(), o => true)); }
        }
        public ICommand RemoveNodeCommand
        {
            get { return _removeNodeCommand ?? (_removeNodeCommand = new RelayCommand(o => RemoveNode(), o => true)); }
        }
        public ICommand InsertNodeCommand
        {
            get {return _insertNodeCommand ?? (_insertNodeCommand = new RelayCommand(o => InsertNode(), o=>true));}
        }

        private void AppendLastLevel()
        {
            if (HierarchyCustom.SelectedTreeNode == null)
                return;
            //var tempClone = HierarchyCustomDefintions.Clone();
            
            //var colCount = tempClone.Columns.Count;
            //var columnName = string.Format("Level{0}", colCount);
            //var dataCol = new DataColumn()
            //{
            //    ColumnName = columnName,
            //    DataType = typeof(string),
            //    DefaultValue = "*"
            //};

            //tempClone.Columns.Add(dataCol);
  
            ////repopulate data
            //foreach (DataRow row in HierarchyCustomDefintions.Rows)
            //    tempClone.ImportRow(row);
                
            //HierarchyCustomDefintions = tempClone;
        }

        private void InsertNode()
        {
            if (EditList.Count == 0 || NewNodeValue == string.Empty)
                return;

            var parent = EditList.First().ParentNode;
            var newNode = new Node() { Value = NewNodeValue, ParentNode = parent};
            parent.AddChild(newNode);

            foreach (var leaf in EditList)
            {
                parent.ChildNodes.Remove(leaf);
                newNode.AddChild(leaf);
                leaf.ParentNode = newNode;
            }

            EditList.Clear();
            NewNodeValue = "";
        }
        private void RemoveNode()
        {
            //stops removing root or altering leafs of the tree
            if (HierarchyCustom.SelectedTreeNode == null || HierarchyCustom.SelectedTreeNode.ParentNode == null ||
                HierarchyCustom.SelectedTreeNode.IsLeaf == true)
                return;
            var node = HierarchyCustom.SelectedTreeNode;
            var parentNode = node.ParentNode;
            var children = node.ChildNodes;

            parentNode.ChildNodes.Remove(node);
            
            foreach(var child in children)
            {
                child.ParentNode = parentNode;
                parentNode.AddChild(child);
            }

        }



        private void RemoveLastLevel()
        {
            if (HierarchyCustom.SelectedTreeNode == null)
                return;
            //var columnCount = HierarchyCustomDefintions.Columns.Count;

            //if (columnCount == 1)
            //    return;

            //var tempClone = HierarchyCustomDefintions.Clone();
            //tempClone.Columns.RemoveAt(columnCount -1);

            ////repopulate data
            //foreach (DataRow row in HierarchyCustomDefintions.Rows)
            //    tempClone.ImportRow(row);

            //HierarchyCustomDefintions = tempClone;
        }
        private void AddItemToEditList()
        {
            if (HierarchyCustom != null && HierarchyCustom.SelectedTreeNode != null)
                AddToEditList(HierarchyCustom.SelectedTreeNode);
        }
        private void InitCustomHierarchy()
        {
            HierarchyCustom = new AnonymisationHierarchy();

            var rootNode = new Node(){Value = "*", LevelDepth = 0};
            HierarchyCustom.RootNode = rootNode;

            foreach(var val in _hierarchyDefintionOptionsVm.UniqueValues)
            {
                var node = new Node() { Value = val, ParentNode = rootNode };
                rootNode.AddChild(node);
            }

        }
        public void GenerateHierarchy()
        {
            InitCustomHierarchy();
            _hierarchyStrRedaction = StringRedactionHierarchyGenerator.GenerateH(_hierarchyDefintionOptionsVm.UniqueValues);

        }
        public void AddToEditList(Node node)
        {
            //Should not edit root
            if (node.ParentNode == null)
                return;

            if (EditList.Count == 0)
            {
                EditList.Add(node);
                return;
            }

            //All must have the same parent
            var parentNode = EditList.First().ParentNode;
            if (!EditList.Contains(node) && parentNode == node.ParentNode)
                EditList.Add(node);
        }
        public void ClearEditList()
        {
            EditList.Clear();
        }

        public AnonymisationHierarchy ExtractHierarchy()
        {
            return ((IsCustomHierarchySelected) ? _hierarchyCustom : _hierarchyStrRedaction);
        }
        private Dictionary<string, LinkedList<string>> CollateCustomHierarchy()
        {
            return null;
            //var result = new Dictionary<string, LinkedList<string>>();

            //if (_hierarchyCustomDefintions == null)
            //    return null;

            //var anonLevels = _hierarchyCustomDefintions.Columns.Count;
            //foreach(DataRow row in HierarchyCustomDefintions.Rows)
            //{
            //    var initVal = row["Level0"].ToString();
            //    var linkList = new LinkedList<string>();
            //    var node = linkList.AddFirst(initVal);


            //    for(int index = 1; index <anonLevels; index++)
            //    {
            //        var colName = string.Format("Level{0}", index);
            //        var nextVal = row[colName].ToString();
            //        node = linkList.AddAfter(node, nextVal);
            //    }

            //    result.Add(initVal, linkList);
            //}

            //return result;
        }
    }
}
