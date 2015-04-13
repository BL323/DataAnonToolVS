using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Core.Output;
using KAnonymisation.UI.Output;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Hierarchy
{
    public class HierarchyBasedAnonymisation : IKAnonymisation
    {
        public string Name
        {
            get { return "Default Hierarchy Based Anonymisation"; }
        }
        public bool RequiresHierarchy
        {
            get { return true; }
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

        private void EditRow(DataRow row, string header, string toUpdate)
        {
            row.BeginEdit();
            row[header] = toUpdate;
            row.EndEdit();
            row.AcceptChanges();
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
        private void AnonymiseQuasiIdentifier(ColumnModel columnModel, ref DataTable dataTable)
        {
            if (columnModel.AnonymisationHierarchy == null)
                throw new Exception("No valid anonymisation hierarchy has been defined.");

            //count occurances of each key
            var valueLookup = new Dictionary<string, int>();

            var valsToBeAnonymised = new List<string>();

            //count occurances and measure valsToBeAnonymised
            valueLookup = CountOccurances(dataTable, columnModel);
            valsToBeAnonymised = FindValuesToBeAnonymised(valueLookup, columnModel.K, columnModel.AnonymisationHierarchy);
            if (valsToBeAnonymised.Count < 1)
                return;

            while(valsToBeAnonymised.Count > 0)
            {
                var newAnonValues = ApplyNextLevelAnonymisation(valsToBeAnonymised, columnModel.AnonymisationHierarchy);
                ApplyAnonymisedValues(newAnonValues, ref dataTable, columnModel);

                valueLookup = CountOccurances(dataTable, columnModel);
                valsToBeAnonymised = FindValuesToBeAnonymised(valueLookup, columnModel.K, columnModel.AnonymisationHierarchy);
            }

        }
        private void ApplyAnonymisedValues(Dictionary<string, string> newAnonValues, ref DataTable dataTable, ColumnModel columnModel)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                var val = row[columnModel.Header].ToString();
                if (newAnonValues.ContainsKey(val))
                    row[columnModel.Header] = newAnonValues[val];
            }

        }

        private List<string> FindValuesToBeAnonymised(Dictionary<string, int> valueLookup, int k, AnonymisationHierarchy anonHierarchy)
        {
            var valsToBeAnonymised = new List<string>();
            var rootVal = anonHierarchy.RootNode.Value;

            foreach (var item in valueLookup)
            {
                if (item.Value < k)
                {
                    for (var count = 0; count < item.Value; count++)
                        if (!valsToBeAnonymised.Contains(item.Key) && !item.Key.Equals(rootVal))
                            valsToBeAnonymised.Add(item.Key);
                }
            }

            return valsToBeAnonymised;
        }

        private Dictionary<string, int> CountOccurances(DataTable dataTable, ColumnModel columnModel)
        {
            var valueLookup = new Dictionary<string, int>();

            foreach (DataRow row in dataTable.Rows)
            {
                var val = row[columnModel.Header].ToString();
                if (valueLookup.ContainsKey(val))
                    valueLookup[val]++;
                else
                    valueLookup.Add(val, 1);
            }

            return valueLookup;
        }
        private Dictionary<string, string> ApplyNextLevelAnonymisation(List<string> valsToBeAnonymised, AnonymisationHierarchy anonymisationHierarchy)
        {
            var result = new Dictionary<string, string>();

            var nodes = new List<Node>();

            if (valsToBeAnonymised == null || anonymisationHierarchy == null)
                Console.WriteLine();

            foreach(var val in valsToBeAnonymised)
                nodes.Add(anonymisationHierarchy.FindNode(val));


            foreach(var node in nodes)
            {
                if (result != null && !result.ContainsKey(node.Value))
                {
                    var parentValue = (node.ParentNode == null) ? null : node.ParentNode.Value;
                    result.Add(node.Value, parentValue);

                }
            }

            return result;
        }
     

    }
}
