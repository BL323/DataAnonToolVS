﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Core.TypeComparer;

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
                if(clustersToApply != null && clustersToApply.Count == 1 && clustersToApply[0].Count == 1)
                {
                    //single value cannot be grouped into a set by itself
                    FullyAnonymiseSingularValue(clustersToApply[0][0].ToString(), ref dataTable, columnModel.Header);
                    return;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    var val = row[columnModel.Header].ToString();
                    if (valsToBeAnonymised.Contains(val))
                        if (clustersToApply != null)
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
                if (clustersToApply != null && clustersToApply.Count == 1 && clustersToApply[0].Count == 1)
                {
                    //single value cannot be grouped into a set by itself
                    FullyAnonymiseSingularValue(clustersToApply[0][0].ToString(), ref dataTable, columnModel.Header);
                    return;
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    var val = int.Parse(row[columnModel.Header].ToString());
                    if (valsToBeAnonymised.Contains(val.ToString()))
                        if (clustersToApply != null)
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
                if (clustersToApply != null && clustersToApply.Count == 1 && clustersToApply[0].Count == 1)
                {
                    //single value cannot be grouped into a set by itself
                    FullyAnonymiseSingularValue(clustersToApply[0][0].ToString(), ref dataTable, columnModel.Header);
                    return;
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    var val = double.Parse(row[columnModel.Header].ToString());
                    if (valsToBeAnonymised.Contains(val.ToString()))
                        if (clustersToApply != null)
                            foreach (var cluster in clustersToApply)
                                if (cluster.Contains(val))
                                {
                                    var newcluster = string.Join(", ", cluster);
                                    var strCluster = "{" + newcluster + "}";
                                    EditRow(row, columnModel.Header, strCluster);

                                }
                }
            }
            else if(columnModel.DataType == typeof(DateTime))
            {
                var listDatesToAnonyise = new List<DateTime>();
                foreach (var str in valsToBeAnonymised)
                    listDatesToAnonyise.Add(DateTime.Parse(str));

                var clustersToApply = ClusterKMembers<DateTime>(columnModel.K, listDatesToAnonyise);
                if (clustersToApply != null && clustersToApply.Count == 1 && clustersToApply[0].Count == 1)
                {
                    //single value cannot be grouped into a set by itself
                    FullyAnonymiseSingularValue(clustersToApply[0][0].ToString(), ref dataTable, columnModel.Header);
                    return;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    var val = DateTime.Parse(row[columnModel.Header].ToString());
                    if (listDatesToAnonyise.Contains(val))
                        if (clustersToApply != null)
                            foreach (var cluster in clustersToApply)
                                if (cluster.Contains(val))
                                {
                                    var strs = new List<string>();
                                    foreach(var d in cluster)
                                        strs.Add(d.ToShortDateString());   

                                    var newcluster = string.Join(", ", strs);
                                    var strCluster = "{" + newcluster + "}";
                                    EditRow(row, columnModel.Header, strCluster);

                                }
                }

            }
        }

        private void FullyAnonymiseSingularValue(string val, ref DataTable dataTable, string columnHeader)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                if (row[columnHeader].ToString() == val)
                    EditRow(row, columnHeader, "*");
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

            //shuffled values via LINQ statment
            var shuffledList = values.OrderBy(val => rand.Next()).ToList();
            
            if (shuffledList.Count < 1)
                return clusters;
                
            var r = shuffledList.First();

            while (shuffledList.Count >= k)
            {
                r = FurthestVal<T>(r, ref shuffledList);
                shuffledList.Remove(r);

                var cluster = new List<T>();
                //cluster container used to count number of items, can't expliclty count occurences of numbers
                //in case the same number is added to the cluster i.e {2, 2, 4} -> {2, 4}
                int clusterContainer = 1;
                cluster.Add(r);

                while (clusterContainer < k)
                {
                    r = NearestVal<T>(r, ref shuffledList);
                    if (!cluster.Contains(r))
                        cluster.Add(r);
                    shuffledList.Remove(r);
                    clusterContainer++;
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
            //generate dictionary with measurement of values            
            var keyDist = CalcSimilairtyDict(r, shuffledArray);
            var furthestVal = keyDist.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return furthestVal;
        }
        private T NearestVal<T>(T r, ref List<T> shuffledArray) where T : IComparable
        {
            //generate dictionary with measurement of values
            var keyDist = CalcSimilairtyDict<T>(r, shuffledArray);
            var nearestVal = keyDist.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
            return nearestVal;
        }
        private Dictionary<T, double> CalcSimilairtyDict<T>(T r, List<T> shuffledArray)
        {
            //maps the attribute data type to associated calculation method
            Dictionary<T, double> result;

            if (typeof(T) == typeof(string))
                result = CalcLevenstheinVals<T>(r, shuffledArray);
            else if (typeof(T) == typeof(int))
                result = CalcIntVals<T>(r, shuffledArray);
            else if (typeof(T) == typeof(double))
                result = CalcDoubleVals<T>(r, shuffledArray);
            else if (typeof(T) == typeof(DateTime))
                result = CalcDateVals<T>(r, shuffledArray);
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
        private Dictionary<T, double> CalcDateVals<T>(T r, List<T> shuffledArray)
        {
            var keyDist = new Dictionary<T, double>();

            foreach (var d in shuffledArray)
            {
                if (!keyDist.ContainsKey(d))
                {
                    var rDate = DateTime.Parse(r.ToString());
                    var date = DateTime.Parse(d.ToString());

                    var res = (rDate - date).TotalDays;
                    var absRes = Math.Abs(res);

                    keyDist.Add(d, absRes);
                }
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
