using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Hierarchy
{
    public class Node : UpdateBase
    {
        private bool _isOpen = true;
        private string _value;
        private Node _parent;
        private ObservableCollection<Node> _childNodes = new ObservableCollection<Node>();

        public bool IsLeaf { get { return ((_childNodes.Count == 0) ? true : false); } }
        public int LevelDepth { get; set; }
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if(_isOpen != value)
                {
                    _isOpen = value;
                    RaisePropertyChanged(() => IsOpen);
                }
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                if(_value != value)
                {
                    _value = value;
                    RaisePropertyChanged(() => Value);
                }
            }
        }
        public Node ParentNode
        {
            get { return _parent; }
            set
            {
                if(_parent != value)
                {
                    _parent = value;
                    RaisePropertyChanged(() => ParentNode);
                }
            }
        }
        public ObservableCollection<Node> ChildNodes
        {
            get { return _childNodes; }
            set
            {
                if (_childNodes != value)
                {
                    _childNodes = value;
                    RaisePropertyChanged(() => ChildNodes);
                }
            }
        }

        public void AddChild(Node childNode)
        {
            _childNodes.Add(childNode);
        }
    }
}
