using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Hierarchy
{
    public class AnonymisationHierarchy : UpdateBase
    {
        private Node _rootNode;
        public Node RootNode
        {
            get { return Nodes.First(); }
            set
            {
                Nodes.Clear();
                Nodes.Add(value);
            }
            //set
            //{
            //    if(_rootNode != value)
            //    {
            //        _rootNode = value;
            //        RaisePropertyChanged(() => RootNode);
            //    }
            //}
        }
        public Node SelectedTreeNode { get; set; }

        private ObservableCollection<Node> _nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get { return _nodes; }
            set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    RaisePropertyChanged(() => Nodes);
                }
            }
        }
 
        public int HierarchyDepth()
        {
            var depth = 0;

            return depth;
        }      
        public List<Node> LeafNodes(Node root)
        {
            if (root == null)
                return null;

            var leafNodes = new List<Node>();

            if (root.IsLeaf)
            {
                leafNodes.Add(root);
                return leafNodes;
            }

            FindLeaves(root, ref leafNodes);


            return leafNodes;
        }

        public Node FindNode(string value)
        {
            Node result = null;

            if (RootNode == null)
                return null;

            if (RootNode.Value == value)
                return RootNode;
            
            
            MatchValues(RootNode, ref result, value);
            return result;            
        }
        private void MatchValues(Node node, ref Node result, string value)
        {
            if(node.ChildNodes != null)
            {
                foreach(var child in node.ChildNodes)
                {
                    if (child.Value == value)
                    {
                        result = child;
                        return;
                    }

                    MatchValues(child, ref result, value);
                }
            }
        }
        private void FindLeaves(Node node, ref List<Node> leaves)
        {
            if(node.ChildNodes != null)
            {
                foreach(var child in node.ChildNodes)
                {
                    if (child.IsLeaf)
                        leaves.Add(child);

                    FindLeaves(child, ref leaves);
                }
            }
        }

        public List<Node> GetAllNodes()
        {
            var result = new List<Node>();

            if (RootNode == null)
                return result;

            result.Add(RootNode);
            FindNodes(RootNode, ref result);

            return result;

        }
        private void FindNodes(Node node, ref List<Node> nodes)
        {
            if(node.ChildNodes != null)
            {
                foreach(var child in node.ChildNodes)
                {
                    nodes.Add(child);
                    FindNodes(child, ref nodes);
                }
            }
        }
    }
}
