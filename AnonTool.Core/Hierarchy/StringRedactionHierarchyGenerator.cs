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
        public static Dictionary<string, LinkedList<string>> Generate(ObservableCollection<string> values)
        {
            var maxLength = values.Max(x => x.Length);

            //pad all strings to maxLength

            var paddedStrList = new List<string>();
            foreach(var str in values)
            {
                var paddedStr = str.PadRight(maxLength, '*');
                paddedStrList.Add(paddedStr);
            }

            var result = new Dictionary<string, LinkedList<string>>();


            for (var index = 0; index < paddedStrList.Count; index++)
            {
                var linkedList = new LinkedList<string>();

                var originalStr = values[index];
                var paddedStr = paddedStrList[index];

                var node = linkedList.AddFirst(paddedStr);

                for (var i = 1; i < maxLength + 1; i++)
                {
                    var alteredStr = paddedStr.Substring(0, paddedStr.Length - i);
                    var redaction = string.Concat(Enumerable.Repeat('*', i));
                    alteredStr = string.Format("{0}{1}", alteredStr, redaction);

                    node = linkedList.AddAfter(node, alteredStr);
                }

                result.Add(originalStr, linkedList);
            }


            return result;
        }
    }
}
