using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Hierarchy
{
    public class ColumnHierarchy
    {
        public Dictionary<string, LinkedList<string>> AnonymistionValues { get; set; }

        public ColumnHierarchy(List<string> uniqueValues)
        {
            AnonymistionValues = new Dictionary<string, LinkedList<string>>();

            foreach (var uv in uniqueValues)
                if (!AnonymistionValues.ContainsKey(uv))
                {
                    var linkedList = new LinkedList<string>();
                    linkedList.AddFirst(uv);
                    AnonymistionValues.Add(uv, linkedList);
                }


        }
    }
}
