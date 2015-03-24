using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnonTool.Core.Hierarchy;
using System.Collections.ObjectModel;

namespace DataAnonTool.Tests
{
    [TestClass]
    public class HierarchyGenerationTests
    {
        [TestMethod]
        public void GenerateFromSingleStringTest()
        {
            var stringsToRedact = new ObservableCollection<string>();
            stringsToRedact.Add("foo bar");

            var resultHierarchy = StringRedactionHierarchyGenerator.Generate(stringsToRedact);
            
            Assert.IsNotNull(resultHierarchy);
            Assert.IsTrue(resultHierarchy.GetAllNodes().Count == 8);
        }

        [TestMethod]
        public void EmptyListTest()
        {
            var stringsToRedact = new ObservableCollection<string>();

            var resultHierarchy = StringRedactionHierarchyGenerator.Generate(stringsToRedact);
            Assert.IsNull(resultHierarchy);
        }

        [TestMethod]
        public void FindNodeChildrenTest()
        {
            var stringsToRedact = new ObservableCollection<string>();
            stringsToRedact.Add("foo bar");
            stringsToRedact.Add("foo bor");

            var resultHierarchy = StringRedactionHierarchyGenerator.Generate(stringsToRedact);

            var node = resultHierarchy.FindNode("foo b**");
            
            Assert.IsNotNull(node);
            Assert.IsTrue(node.ChildNodes.Count == 2);

        }
    }
}
