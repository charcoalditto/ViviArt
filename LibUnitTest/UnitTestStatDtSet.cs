using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViviArt;

namespace LibUnitTest
{
    [TestClass]
    public class UnitTestStatDtSet
    {
        [TestMethod]
        public void TestStatDtRangeDay()
        {
            var res = DateTimeLib.StatDtRange("d", new DateTime(2018, 1, 1, 1, 1, 1), new DateTime(2018, 1, 4));
            var cmp = new List<DateTime>(){
                new DateTime(2018, 1, 1),
                new DateTime(2018, 1, 2),
                new DateTime(2018, 1, 3),
            };
            int i = 0;
            foreach(var row in res)
            {
                Assert.AreEqual(cmp[i], row);
                i += 1;
            }
            Assert.AreEqual(cmp.Count, i);
        }
        [TestMethod]
        public void TestStatDtRangeWeek1()
        {
            var res = DateTimeLib.StatDtRange("w", new DateTime(2018, 1, 1, 1, 1, 1), new DateTime(2018, 1, 8));
            var cmp = new List<DateTime>(){
                new DateTime(2018, 1, 1),
            };
            int i = 0;
            foreach (var row in res)
            {
                Assert.AreEqual(cmp[i], row);
                i += 1;
            }
            Assert.AreEqual(cmp.Count, i);
        }
        [TestMethod]
        public void TestStatDtRangeWeek2()
        {
            var res = DateTimeLib.StatDtRange("w", new DateTime(2018, 1, 1, 1, 1, 1), new DateTime(2018, 1, 18));
            var cmp = new List<DateTime>(){
                new DateTime(2018, 1, 1),
                new DateTime(2018, 1, 8),
            };
            int i = 0;
            foreach (var row in res)
            {
                Assert.AreEqual(cmp[i], row);
                i += 1;
            }
            Assert.AreEqual(cmp.Count, i);
        }
        [TestMethod]
        public void TestStatDtRangeWeek3()
        {
            var res = DateTimeLib.StatDtRange("w", new DateTime(2018, 1, 18), new DateTime(2018, 2, 1));
            var cmp = new List<DateTime>(){
                new DateTime(2018, 1, 15),
                new DateTime(2018, 1, 22),
            };
            int i = 0;
            foreach (var row in res)
            {
                Assert.AreEqual(cmp[i], row);
                i += 1;
            }
            Assert.AreEqual(cmp.Count, i);
        }
        [TestMethod]
        public void TestStatDtRangeMonth1()
        {
            var res = DateTimeLib.StatDtRange("m", new DateTime(2018, 1, 14), new DateTime(2018, 4, 1));
            var cmp = new List<DateTime>(){
                new DateTime(2018, 1, 1),
                new DateTime(2018, 2, 1),
                new DateTime(2018, 3, 1),
            };
            int i = 0;
            foreach (var row in res)
            {
                Assert.AreEqual(cmp[i], row);
                i += 1;
            }
            Assert.AreEqual(cmp.Count, i);
        }
        [TestMethod]
        public void TestStatDtRangeYear1()
        {
            var res = DateTimeLib.StatDtRange("y", new DateTime(2018, 1, 14), new DateTime(2020, 4, 1));
            var cmp = new List<DateTime>(){
                new DateTime(2018, 1, 1),
                new DateTime(2019, 1, 1),
            };
            int i = 0;
            foreach (var row in res)
            {
                Assert.AreEqual(cmp[i], row);
                i += 1;
            }
            Assert.AreEqual(cmp.Count, i);
        }
    }
}
