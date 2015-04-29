using System.Data;
using KAnonymisation.Core.ColumnInfo;

namespace KAnonymisation.Core.Interfaces
{
    public interface IKAnonymisation
    {
        //ReadOnly Properties
        string Name { get; }
        bool RequiresHierarchy { get; }  
        //Methods
        void ApplyAnonymisation(ref DataTable dataTable, ColumnModel columnModel);
    }
}
