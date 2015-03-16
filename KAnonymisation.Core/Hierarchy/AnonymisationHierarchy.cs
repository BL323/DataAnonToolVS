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
