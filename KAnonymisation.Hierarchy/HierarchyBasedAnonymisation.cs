using KAnonymisation.Core.ColumnInfo;
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
        public void Show()
        {
            throw new NotImplementedException();
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

            var listOfLinkedLists = new List<Tuple<string, LinkedList<string>>>();
            foreach(var ll in columnModel.ColumnHierarchy.AnonymistionValues)
                listOfLinkedLists.Add(new Tuple<string, LinkedList<string>>(ll.Key, ll.Value));
            

            //count occurances of each key
            var valueLookup = new Dictionary<string, int>();


            var valsToBeAnonymised = new List<string>();


            //count occurances and measure valsToBeAnonymised
            valueLookup = CountOccurances(dataTable, columnModel);
            valsToBeAnonymised = FindValuesToBeAnonymised(valueLookup, columnModel.K);
            if (valsToBeAnonymised.Count < 1)
                return;

            while (valsToBeAnonymised.Count > 0)
            {
                var newAnonValues = AnonymiseValuesToNextLevel(valsToBeAnonymised, ref listOfLinkedLists);
                ApplyAnonymisedValues(newAnonValues, ref dataTable, columnModel);
                
                valueLookup = CountOccurances(dataTable, columnModel);
                valsToBeAnonymised = FindValuesToBeAnonymised(valueLookup, columnModel.K);
            }
           // var newAnonValues = AnonymiseValuesToNextLevel(valsToBeAnonymised, ref listOfLinkedLists);
           // ApplyAnonymisedValues(newAnonValues, ref dataTable, columnModel);
        }

        private Dictionary<string, string> AnonymiseValuesToNextLevel(List<string> valsToBeAnonymised, ref List<Tuple<string, LinkedList<string>>> listOfLinkedLists)
        {
            //dict old val and new val
            var result = new Dictionary<string, string>();

            for (var index = 0; index < valsToBeAnonymised.Count; index++)
            {
                var ldl = listOfLinkedLists.Find(x => x.Item2.First() == valsToBeAnonymised[index]);
                if (ldl == null)
                    ldl = listOfLinkedLists.Find(x => x.Item2.First().Contains(valsToBeAnonymised[index]));

                listOfLinkedLists.Remove(ldl);

                var firstNode = ldl.Item2.First;
                var curStr = ldl.Item1;
                var newNode = firstNode.Next;
                var newStr = newNode.Value;

                valsToBeAnonymised[index] = newStr;

                ldl.Item2.RemoveFirst();
                listOfLinkedLists.Add(new Tuple<string, LinkedList<string>>(newStr, ldl.Item2));

                if (result.ContainsKey(curStr))
                    result.Remove(curStr);

                result.Add(curStr, newStr);
            }

            return result;
        }

        private void ApplyAnonymisedValues(Dictionary<string, string> newAnonValues, ref DataTable dataTable, ColumnModel columnModel)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                var val = row[columnModel.Header].ToString();
                if (newAnonValues.ContainsKey(val))
                    row[columnModel.Header] = newAnonValues[val];
            }

        }
        private List<string> FindValuesToBeAnonymised(Dictionary<string, int> valueLookup, int k)
        {
            var valsToBeAnonymised = new List<string>();

            foreach (var item in valueLookup)
            {
                if (item.Value < k)
                {
                    for (var count = 0; count < item.Value; count++)
                        if(!valsToBeAnonymised.Contains(item.Key))
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
        private Dictionary<string, string> AnonymiseValuesToNextLevel(List<string> valsToBeAnonymised, ref List<LinkedList<string>> listOfLinkedLists)
        {
            //dict old val and new val
            var result = new Dictionary<string, string>();

            for(var index = 0; index < valsToBeAnonymised.Count; index++)
            {
                var ldl = listOfLinkedLists.Find(x => x.First() == valsToBeAnonymised[index]);
                if (ldl == null)
                    ldl = listOfLinkedLists.Find(x => x.First().Contains(valsToBeAnonymised[index]));

                listOfLinkedLists.Remove(ldl);

                var firstNode = ldl.First;
                var curStr = firstNode.Value;
                var newNode = firstNode.Next;
                var newStr = newNode.Value;

                valsToBeAnonymised[index] = newStr;

                ldl.RemoveFirst();
                listOfLinkedLists.Add(ldl);

                result.Add(curStr, newStr);
            }

            return result;
        }


        //helper methods
        private void EditRow(DataRow row, string header, string toUpdate)
        {
            row.BeginEdit();
            row[header] = toUpdate;
            row.EndEdit();
            row.AcceptChanges();
        }
    }
}
