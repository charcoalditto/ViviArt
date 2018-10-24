using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViviArt;

namespace LibUnitTest
{
    [TestClass]
    public class UnitTestStatDt
    {

        [TestMethod]
        public void TestStatDtDay1()
        {
            var dt1 = (new DateTime(2018, 1, 1)).StatDt("d");
            Assert.AreEqual(new DateTime(2018, 1, 1), dt1);
        }

        [TestMethod]
        public void TestStatDtDay2()
        {
            var dt1 = (new DateTime(2018, 1, 1, 1, 1, 1)).StatDt("d");
            Assert.AreEqual(new DateTime(2018, 1, 1), dt1);
        }
        [TestMethod]
        public void TestStatDtWeek1()
        {
            var dt1 = (new DateTime(2018, 9, 24)).StatDt("w");
            Assert.AreEqual(new DateTime(2018, 9, 24), dt1);
        }
        [TestMethod]
        public void TestStatDtWeek3()
        {
            var dt1 = (new DateTime(2018, 9, 30, 23, 59, 59)).StatDt("w");
            Assert.AreEqual(new DateTime(2018, 9, 24), dt1);
        }
        [TestMethod]
        public void TestStatDtWeek4()
        {
            var dt1 = (new DateTime(2018, 10, 1)).StatDt("w");
            Assert.AreEqual(new DateTime(2018, 10, 1), dt1);
        }
        [TestMethod]
        public void TestStatDtMonth1()
        {
            var dt1 = (new DateTime(2018, 9, 1)).StatDt("m");
            Assert.AreEqual(new DateTime(2018, 9, 1), dt1);
        }
        [TestMethod]
        public void TestStatDtMonth2()
        {
            var dt1 = (new DateTime(2018, 9, 30, 23, 59, 59)).StatDt("m");
            Assert.AreEqual(new DateTime(2018, 9, 1), dt1);
        }
        [TestMethod]
        public void TestStatDtYear1()
        {
            var dt1 = (new DateTime(2018, 9, 30)).StatDt("y");
            Assert.AreEqual(new DateTime(2018, 1, 1), dt1);
        }
        [TestMethod]
        public void TestStatDtYear2()
        {
            var dt1 = (new DateTime(2018, 1, 1)).StatDt("y");
            Assert.AreEqual(new DateTime(2018, 1, 1), dt1);
        }
    }
}
