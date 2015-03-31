using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Core.Output;
using KAnonymisation.Core.TypeComparer;
using KAnonymisation.UI.Output;
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
            var numRows = dataTable.Rows.Count;

            if (columnModel.K > numRows)
                throw new Exception("Error - K level cannot be greater than number of data entries.");

            //count occurances of each key
            var valueLookup = new Dictionary<string, int>();
            foreach (DataRow row in dataTable.Rows)
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
                    for (var count = 0; count < item.Value; count++)
                        valsToBeAnonymised.Add(item.Key);
                }
            }

            //establish sets from keys that do not meet threshold
            //generate set based on set 

            if (valsToBeAnonymised.Count < 1)
                return;
  
            if (columnModel.DataType == typeof(string))
            {
                var clustersToApply = ClusterKMembers<string>(columnModel.K, valsToBeAnonymised);
                foreach (DataRow row in dataTable.Rows)
                {
                    var val = row[columnModel.Header].ToString();
                    if (valsToBeAnonymised.Contains(val))
                        foreach (var cluster in clustersToApply)
                            if (cluster.Contains(val))
                            {
                                var newcluster = string.Join(", ", cluster);
                                var strCluster = "{" + newcluster + "}";
                                EditRow(row, columnModel.Header, strCluster);
                            }
                }
            }
            else if (columnModel.DataType == typeof(int))
            {
                var listIntsToAnonymise = new List<int>();
                foreach (var str in valsToBeAnonymised)
                    listIntsToAnonymise.Add(int.Parse(str));

                var clustersToApply = ClusterKMembers<int>(columnModel.K, listIntsToAnonymise);
                foreach (DataRow row in dataTable.Rows)
                {
                    var val = int.Parse(row[columnModel.Header].ToString());
                    if (valsToBeAnonymised.Contains(val.ToString()))
                        foreach (var cluster in clustersToApply)
                            if (cluster.Contains(val))
                            {
                                var newcluster = string.Join(", ", cluster);
                                var strCluster =  "{" + newcluster + "}";
                                EditRow(row, columnModel.Header, strCluster);

                            }
                }

            }
            else if(columnModel.DataType == typeof(double))
            {
                var listIntsToAnonymise = new List<double>();
                foreach (var str in valsToBeAnonymised)
                    listIntsToAnonymise.Add(double.Parse(str));

                var clustersToApply = ClusterKMembers<double>(columnModel.K, listIntsToAnonymise);
                foreach (DataRow row in dataTable.Rows)
                {
                    var val = double.Parse(row[columnModel.Header].ToString());
                    if (valsToBeAnonymised.Contains(val.ToString()))
                        foreach (var cluster in clustersToApply)
                            if (cluster.Contains(val))
                            {
                                var newcluster = string.Join(", ", cluster);
                                var strCluster = "{" + newcluster + "}";
                                EditRow(row, columnModel.Header, strCluster);

                            }
                }
            }
        }
        private void EditRow(DataRow row, string header,string toUpdate)
        {
            row.BeginEdit();
            row[header] = toUpdate;
            row.EndEdit();
            row.AcceptChanges();
        }

        private List<List<T>> ClusterKMembers<T>(int k, List<T> values) where T : IComparable
        {
            var clusters = new List<List<T>>();
            var rand = new Random();

            //shuffle vals

            var shuffledList = values.OrderBy(val => rand.Next()).ToList();
            
            if (shuffledList.Count < 1)
                return clusters;

            var r = shuffledList.First();
           // shuffledList.Remove(r);

            while (shuffledList.Count > k)
            {
                r = FurthestVal<T>(r, ref shuffledList);
                shuffledList.Remove(r);

                var cluster = new List<T>();
                cluster.Add(r);

                while (cluster.Count < k)
                {
                    r = NearestVal<T>(r, ref shuffledList);
                    if (!cluster.Contains(r))
                        cluster.Add(r);
                    shuffledList.Remove(r);
                }
                clusters.Add(cluster);
            }

            while (shuffledList.Count > 0)
            {
                r = shuffledList.First();
                var nearestCluster = NearestCluster<T>(r, ref clusters);
                if (!nearestCluster.Contains(r))
                  nearestCluster.Add(r);
                shuffledList.Remove(r);
            }

            return clusters;
        }
        private T FurthestVal<T>(T r, ref List<T> shuffledArray) where T : IComparable 
        {
            var keyDist = CalcSimilairtyDict(r, shuffledArray);
            var furthestVal = keyDist.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return furthestVal;
        }
        private T NearestVal<T>(T r, ref List<T> shuffledArray) where T : IComparable
        {
            //generate ditionary with measurement of values
            var keyDist = CalcSimilairtyDict<T>(r, shuffledArray);
            var nearestVal = keyDist.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
            return nearestVal;
        }
        private Dictionary<T, double> CalcSimilairtyDict<T>(T r, List<T> shuffledArray)
        {
            var result = new Dictionary<T, double>();

            if (typeof(T) == typeof(string))
                result = CalcLevenstheinVals<T>(r, shuffledArray);
            else if (typeof(T) == typeof(int))
                result = CalcIntVals<T>(r, shuffledArray);
            else if(typeof(T) == typeof(double))
                result = CalcDoubleVals<T>(r, shuffledArray);
            else
                throw new NotSupportedException();          

            return result;
        }

        private Dictionary<T, double> CalcDoubleVals<T>(object r, List<T> shuffledArray)
        {
            var keyDist = new Dictionary<T, double>();

            foreach (var num in shuffledArray)
            {
                if (!keyDist.ContainsKey(num))
                {
                    double rDec = double.Parse(r.ToString());
                    double numDec = double.Parse(num.ToString());
                    keyDist.Add(num, Math.Abs(rDec - numDec));
                }

            }

            return keyDist;
        }
        private Dictionary<T, double> CalcIntVals<T>(T r, List<T> shuffledArray)
        {
            var keyDist = new Dictionary<T, double>();

            foreach(var num in shuffledArray)
            {
                if (!keyDist.ContainsKey(num))
                {
                    int rInt = int.Parse(r.ToString());
                    int numInt = int.Parse(num.ToString());
                    keyDist.Add(num, Math.Abs(rInt - numInt));
                }

            }

            return keyDist;
        }
        private Dictionary<T, double> CalcLevenstheinVals<T>(T r, List<T> shuffledArray)
        {
            var keyDist = new Dictionary<T, double>();

            foreach (var str in shuffledArray)
            {
                if (!keyDist.ContainsKey(str))
                        keyDist.Add(str, LevenshteinDistance.Compute(r.ToString(), str.ToString()));
            }

            return keyDist;
        }
        private List<T> NearestCluster<T>(T r, ref List<List<T>> clusters) where T : IComparable 
        {
            if(clusters.Count == 0)
            {
                var cluster = new List<T>();
                clusters.Add(cluster);
                return cluster;
            }

            var clusterAvg = -1.0;
            var closestClusterIndex = 0;

            for (var clusterIndex = 0; clusterIndex < clusters.Count; clusterIndex++)
            {
                var dictVals = CalcSimilairtyDict<T>(r, clusters[clusterIndex]);
                var tot = 0.0;
                for (var i = 0; i < dictVals.Count; i++)
                    tot += dictVals.ElementAt(i).Value;
                

                var curAvg = tot / (double)dictVals.Count;
                if (clusterAvg == -1.0)
                    clusterAvg = curAvg;

                if(clusterAvg > curAvg)
                {
                    clusterAvg = curAvg;
                    closestClusterIndex = clusterIndex;
                }
            }

            var closestCluster = clusters[closestClusterIndex];
            return closestCluster;
        }
    }
}
