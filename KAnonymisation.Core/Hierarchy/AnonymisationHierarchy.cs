using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Hierarchy
{
    public class AnonymisationHierarchy 
    {
        public Node RootNode { get; set; }
 
        public int HierarchyDepth()
        {
            var depth = 0;

            return depth;
        }  
        
        public List<Node> LeafNodes()
        {
            if(RootNode == null)
                return null;

            var leafNodes = new List<Node>();

            if (RootNode.IsLeaf)
            {
                leafNodes.Add(RootNode);
                return leafNodes;
            }

            FindLeaves(RootNode, ref leafNodes);


            return leafNodes;
        }
        public Node FindNode(string value)
        {
            Node result = null;

            if (RootNode == null)
                return null;

            if (RootNode.IsLeaf)
            {
                if (RootNode.Value == value)
                    return RootNode;
            }
            
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

    }
}
