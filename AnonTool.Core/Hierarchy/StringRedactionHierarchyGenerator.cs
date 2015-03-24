using KAnonymisation.Core.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Hierarchy
{
    public static class StringRedactionHierarchyGenerator
    {
        public static AnonymisationHierarchy Generate(ObservableCollection<string> values)
        {
            if (values.Count == 0)
                return null;

            var hierarchy = new AnonymisationHierarchy();

            var depth = 1;
            var maxLength = values.Max(x => x.Length);

            //pad all strings to maxLength
            var paddedStrList = new List<string>();
            foreach (var str in values)
            {
                var paddedStr = str.PadRight(maxLength, '*');
                paddedStrList.Add(paddedStr);
            }

            var rootVal = string.Concat(Enumerable.Repeat('*', maxLength));
            
            hierarchy.RootNode = new Node() { LevelDepth = depth, Value = rootVal };


            for (var level = maxLength-1; level > 0; level--)
            {
                var childStrings = RedactStrings(paddedStrList, level);
                AddAnonymisationLevel(ref hierarchy , childStrings);
            }


            //add original strings
            var leafNodes = hierarchy.LeafNodes(hierarchy.RootNode);
            foreach(var leaf in leafNodes)
            {
                var leafStripped = leaf.Value.TrimEnd('*');
                foreach(var val in values)
                {
                    if (val.StartsWith(leafStripped))
                    {
                        var node = new Node() { Value = val, ParentNode = leaf };
                        leaf.AddChild(node);
                    }
                }
            }


            
            return hierarchy;
        }

        private static List<string> RedactStrings(List<string> values, int level)
        {
            var list = new List<string>();

            foreach(var val in values)
            {
                var alteredStr = val.Substring(0, val.Length - level);
                var redaction = string.Concat(Enumerable.Repeat('*', level));
                alteredStr = string.Format("{0}{1}", alteredStr, redaction);

                if (!list.Contains(alteredStr))
                    list.Add(alteredStr);
            }

            return list;
        }
       
        private static void AddAnonymisationLevel(ref AnonymisationHierarchy hierarchy, List<string> children)
        {
            List<Node> leafNodes = hierarchy.LeafNodes(hierarchy.RootNode);

            foreach (var leafNode in leafNodes)
            {
                var nodesToAdd = new List<Node>();
                foreach(var child in children)
                {
                    var strippedLeafValue = leafNode.Value.TrimEnd('*');
                    var strippedNewChild = child.TrimEnd('*');


                    if(strippedNewChild.StartsWith(strippedLeafValue))
                    {
                        var node = new Node() { Value = child, ParentNode = leafNode };
                        leafNode.AddChild(node);
                    }
                }

            }

        }
    }
}
