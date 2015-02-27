using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Core.TypeComparer;
using System;
using System.Collections.Generic;
using System.Data;
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
        public void Anonymise(DataTable dataTable, List<ColumnModel> columnsInfo)
        {
            //Annonymise Data
            foreach(var column  in columnsInfo)            
                AnonymiseColumn(column, ref dataTable);

            //Display Data
        }

        private void AnonymiseColumn(ColumnModel column, ref DataTable dataTable)
        {
            switch(column.AttributeType)
            {
                case IdentifierType.Explicit:
                    AnonymiseExplicitIdentifier(column, ref dataTable);
                    break;
                case IdentifierType.Quasi:
                    AnonymiseQuasiIdentifier(column, ref dataTable);
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
            foreach(DataRow row in dataTable.Rows)
            {
                row.BeginEdit();
                row[columnModel.Header] = "*";
                row.EndEdit();
                row.AcceptChanges();
            }

        }
    
        private void AnonymiseQuasiIdentifier(ColumnModel columnModel, ref DataTable dataTable)
        {
            //count occurances of each key
            var valueLookup = new Dictionary<string, int>();
            foreach(DataRow row in dataTable.Rows)
            {
                var val = row[columnModel.Header].ToString();
                if (valueLookup.ContainsKey(val))
                    valueLookup[val]++;
                else
                    valueLookup.Add(val, 1);
            }

            var valsToBeAnonymised = new List<string>();

            foreach (var item in valueLookup)
            {
                if (item.Value < columnModel.K)
                {
                    for(var count = 0; count < item.Value; count++)
                        valsToBeAnonymised.Add(item.Key);
                }
            }

            //establish sets from keys that do not meet threshold
            //generate set based on set 
            ClusterKMembersString(columnModel.K, valsToBeAnonymised);
        }

        private void ClusterKMembersString(int k, List<string> values)
        {
            var clusters = new List<List<string>>();
            var rand = new Random();

            //shuffle values
            var shuffledArray = values.OrderBy(val => rand.Next()).ToList();

            var r = shuffledArray.First();
            shuffledArray.Remove(r);

            while(shuffledArray.Count > k)
            {
                r = FurthestVal(r, ref shuffledArray);
                shuffledArray.Remove(r);


                var cluster = new List<string>();
                cluster.Add(r);

                //init row
                while(cluster.Count < k)
                {
                    r = NearestVal(r, ref shuffledArray);
                    cluster.Add(r);
                    shuffledArray.Remove(r);
                }
                clusters.Add(cluster);
            }

            while(shuffledArray.Count > 0)
            {
                r = shuffledArray.First();
                var nearestCluster = NearestCluster(r, ref clusters);
                //nearestCluster.Add(r)
                shuffledArray.Remove(r);
            }

        }

        private List<string> NearestCluster(string r, ref List<List<string>> clusters)
        {
            foreach(List<string> cluster in clusters)
            {
                var dict = CalcLevenstheinVals(r, cluster);

            }
            return null;
        }

        private Dictionary<string, int> CalcLevenstheinVals(string r, List<string> shuffledArray)
        {
            var keyDist = new Dictionary<string, int>();

            foreach (var str in shuffledArray)
                keyDist.Add(str, LevenshteinDistance.Compute(r, str));

            return keyDist;
        }
        private string NearestVal(string r, ref List<string> shuffledArray)
        {
            var keyDist = CalcLevenstheinVals(r, shuffledArray);
            var nearestVal = keyDist.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;

            return nearestVal;
        }
        private string FurthestVal(string r, ref List<string> shuffledArray)
        {
            var keyDist = CalcLevenstheinVals(r, shuffledArray);
            var furthestVal = keyDist.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            return furthestVal;
        }
    }
}
