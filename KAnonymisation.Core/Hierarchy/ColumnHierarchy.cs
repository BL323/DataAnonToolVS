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
    }
}
