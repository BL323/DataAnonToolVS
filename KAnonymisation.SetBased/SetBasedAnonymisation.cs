using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Core.TypeComparer;
using KAnonymisation.SetBased.Show.ViewModels;
using KAnonymisation.SetBased.Show.Views;
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
            //Set all columns to string data type...[11-18] or {12, 16}
            DataTable dtClone = dataTable.Clone();
            foreach(DataColumn column in dataTable.Columns)
                dtClone.Columns[column.ColumnName].DataType = typeof(string);
            foreach (DataRow row in dataTable.Rows)
                dtClone.ImportRow(row);
            dataTable = dtClone;


            //Annonymise Data
            foreach(var column  in columnsInfo)            
                AnonymiseColumn(column, ref dataTable);

            //Display Data
            var resultVm = new ResultViewModel();
            resultVm.OutputDataTable = dataTable;
            var resultDialog = new ResultWindowView();
            resultDialog.DataContext = resultVm;
            resultDialog.Show();

            
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
                            if(cluster.Contains(val))
                                EditRow(row, columnModel.Header, string.Join(", ", cluster));
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
                            if(cluster.Contains(val))
                                EditRow(row, columnModel.Header, string.Join(", ", cluster));
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
            shuffledList.Remove(r);

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
            else
                throw new NotSupportedException();          

            return result;
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
