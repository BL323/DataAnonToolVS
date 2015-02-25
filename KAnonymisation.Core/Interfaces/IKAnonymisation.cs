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
        
        //Methods
        void Show();
        void Anonymise(DataTable dataTable, List<ColumnModel> columnsInfo);

    }
}
