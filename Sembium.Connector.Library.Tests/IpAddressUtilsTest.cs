using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sembium.Connector.Library.Tests
{
    [TestClass]
    public class IPAddressUtilsTest
    {
        [TestMethod]
        public void TestIPAddressMatchesPattern1()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.5";
            var ipAddressPattern = "231.114.0.0";
            var expected = false;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIPAddressMatchesPattern2()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.5";
            var ipAddressPattern = "231.114.3.4/16";
            var expected = true;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIPAddressMatchesPattern3()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.5";
            var ipAddressPattern = "231.114.92.5";
            var expected = true;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIPAddressMatchesPattern4()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.5";
            var ipAddressPattern = "11.114.92.5/11";
            var expected = false;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIPAddressMatchesPattern5()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.5";
            var ipAddressPattern = "231.114.92.5/32";
            var expected = true;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIPAddressMatchesPattern6()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.5";
            var ipAddressPattern = "231.114.92.55/32";
            var expected = false;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestIPAddressMatchesPattern7()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.555";
            var ipAddressPattern = "231.114.92.55/32";
            var expected = false;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIPAddressMatchesPattern8()
        {
            var utils = new Data.Connection.IpAddressUtils();

            var ipAddress = "231.114.92.55";
            var ipAddressPattern = "231.114.92.55/132";
            var expected = false;
            var actual = utils.IpAddressMatchesPattern(ipAddress, ipAddressPattern);

            Assert.AreEqual(expected, actual);
        }
    }
}
