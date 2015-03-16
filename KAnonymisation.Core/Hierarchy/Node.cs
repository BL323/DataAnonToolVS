using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Hierarchy
{
    public class Node
    {
        private List<Node> _childNodes = new List<Node>();

        public bool IsLeaf { get { return ((_childNodes.Count == 0) ? true : false); } }
        public int LevelDepth { get; set; }
        public string Value { get; set; }
        public Node ParentNode { get; set; }
        public List<Node> ChildNodes
        {
            get { return _childNodes; }
            set
            {
                if (_childNodes != value)
                {
                    _childNodes = value;
                }
            }
        }

        public void AddChild(Node childNode)
        {
            _childNodes.Add(childNode);
        }
    }
}
