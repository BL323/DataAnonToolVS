using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KAnonymisation.Core.TypeComparer;

namespace DataAnonTool.Tests
{
    [TestClass]
    public class LevenshteinDistanceTests
    {
        [TestMethod]
        public void EqualStringsTests()
        {
            var str1 = "foo";
            var str2 = "foo";

            var result = LevenshteinDistance.Compute(str1, str2);
            Assert.IsTrue(result == 0.0);
        }

        [TestMethod]
        public void DifferentStringsTest()
        {
            var str1 = "foo";
            var str2 = "bar";

            var result = LevenshteinDistance.Compute(str1, str2);
            Assert.IsTrue(result == 3.0);
        }

        [TestMethod]
        public void EmptyStringTest()
        {
            var str1 = string.Empty;
            var str2 = "foo";

            var result = LevenshteinDistance.Compute(str1, str2);
            Assert.IsTrue(result == 3.0);
        }
    }
}
