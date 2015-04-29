using System;
using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;

namespace KAnonymisation.Core.ColumnInfo
{
    public class ColumnModel
    {
        public ColumnModel()
        {
            K = 0;
        }
        public int K { get; set; }
        public string Header { get; set; }
        public Type DataType { get; set; }
        public IdentifierType AttributeType { get; set; }
        public AnonymisationHierarchy AnonymisationHierarchy { get; set; }
        public IKAnonymisation KAnonymisation { get; set; }
    }
}
