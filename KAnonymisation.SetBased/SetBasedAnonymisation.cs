using KAnonymisation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.SetBased
{
    public class SetBasedAnonymisation : IKAnonymisation
    {
        public string Name
        {
            get { return "Default Set Based Anonymisation"; }
        }

        public void Show()
        {
            throw new NotImplementedException();
        }
        public void Anonymise(System.Data.DataTable dataTable, List<Core.ColumnInfo.ColumnModel> columnsInfo)
        {
            throw new NotImplementedException();
        }
    }
}
