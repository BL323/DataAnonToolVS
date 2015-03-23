using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.PluginPOC
{
    public class PluginProofOfConceptAnonymisation : IKAnonymisation
    {
        public string Name
        {
            get { return "Plugin Proof of Concept Anonymisation"; }
        }
        public bool RequiresHierarchy
        {
            get { return false; }
        }

        public void ApplyAnonymisation(ref DataTable dataTable, ColumnModel columnModel)
        {
            switch (columnModel.AttributeType)
            {
                case IdentifierType.Explicit:
                    AnonymiseExplicitIdentifier(columnModel, ref dataTable);
                    break;
                case IdentifierType.Quasi:
                    AnonymiseQuasiIdentifier(columnModel, ref dataTable);
                    break;
                case IdentifierType.Sensitive:
                    break;
                case IdentifierType.NonSensitive:
                    break;
            }
        }

        private void AnonymiseQuasiIdentifier(ColumnModel columnModel, ref DataTable dataTable)
        {
            //Replaces all enteries with POC as proof of concept for plugin anonymisation
            foreach (DataRow row in dataTable.Rows)
            {
                row.BeginEdit();
                //Proof of Concept
                row[columnModel.Header] = "POC";
                row.EndEdit();
                row.AcceptChanges();
            }
        }
        private void AnonymiseExplicitIdentifier(ColumnModel columnModel, ref DataTable dataTable)
        {
            //Replaces all enteries with a * to remove explicit identifiers
            foreach (DataRow row in dataTable.Rows)
            {
                row.BeginEdit();
                row[columnModel.Header] = "*";
                row.EndEdit();
                row.AcceptChanges();
            }
        }
    }
}
