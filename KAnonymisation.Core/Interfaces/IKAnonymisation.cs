using KAnonymisation.Core.ColumnInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.Interfaces
{
    public interface IKAnonymisation
    {
        //Properties
        string Name { get; }
        bool RequiresHierarchy { get; }        
        void ApplyAnonymisation(ref DataTable dataTable, ColumnModel columnModel);
    }
}
