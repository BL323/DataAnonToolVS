﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;

namespace AnonTool.Core.Hierarchy
{
    public class HierarchyDefintionVm : UpdateBase
    {
        //Private Fields    
        private bool _isCustomHierarchySelected;
        private string _newNodeValue;
        private string _matchString;
        private string _selectedRedactionDirection = "Right to Left";
        private AnonymisationHierarchy _hierarchyStrRedaction;
        private AnonymisationHierarchy _hierarchyCustom; 
        private HierarchyDefinitionOptionsVm _hierarchyDefintionOptionsVm = new HierarchyDefinitionOptionsVm();
        private ObservableCollection<Node> _editList = new ObservableCollection<Node>();
        private ICommand _addToEditListCommand;
        private ICommand _clearEditListCommand;
        private ICommand _insertNodeCommand;
        private ICommand _removeNodeCommand;
        private ICommand _addMatchesToEditListCommand;

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
        public string MatchString
        {
            get { return _matchString; }
            set
            {
                if(_matchString != value)
                {
                    _matchString = value;
                    RaisePropertyChanged(() => MatchString);
                }
            }
        }
        public string SelectedRedactionDirection
        {
            get{return _selectedRedactionDirection;}
            set
            {
                if(_selectedRedactionDirection != value)
                {
                    _selectedRedactionDirection = value;
                    GenerateHierarchies();
                    RaisePropertyChanged(() => SelectedRedactionDirection);
                    RaisePropertyChanged(() => HierarchyStrRedaction);
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
        public ObservableCollection<String> AvialableRedactionsDirections
        {
            get { return new ObservableCollection<string>(){"Right to Left", "Left to Right"}; }
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
        public ICommand AddMatchesToEditListCommand
        {
            get { return _addMatchesToEditListCommand ?? (_addMatchesToEditListCommand = new RelayCommand(o => AddMatchesToEditList(), o => true)); }
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
        private void AddMatchesToEditList()
        {
            if (EditList.Count == 0 || MatchString == string.Empty)
                return;

            //Must have common parent
            var parentNode = EditList.First().ParentNode;

            var nodeList = _hierarchyCustom.GetAllNodes();
            //linq statement - find matching items to add in edit list
            var filteredNodes = nodeList.Where(x => x.ParentNode == parentNode 
                                && x.Value.StartsWith(MatchString) && !EditList.Contains(x));

            //alter field, to stop binding refreshing each time an item is added
            foreach (var node in filteredNodes)
                _editList.Add(node);

            RaisePropertyChanged(() => EditList);
            MatchString = "";
                      
        }
        public void GenerateHierarchy()
        {
            InitCustomHierarchy();
            GenerateHierarchies();
        }

        private void GenerateHierarchies()
        {
            _hierarchyStrRedaction = (_selectedRedactionDirection.Equals("Right to Left")) ? StringRedactionHierarchyGenerator.Generate(_hierarchyDefintionOptionsVm.UniqueValues) :
                                                              StringRedactionHierarchyGenerator.GenerateLeftToRight(_hierarchyDefintionOptionsVm.UniqueValues);
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
    }
}
