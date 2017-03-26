using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeePassRPC;
using DomainPublicSuffix;

namespace KeePassRPCTest
{
    [TestClass]
    public class URLSummaryTest
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TLDRulesCache.Init(@"C:\temp\publicSuffixDomainCache.txt");
        }

        [TestMethod]
        public void StandardHttpWithPath()
        {
            var summary = URLSummary.FromURL("http://www.google.com/any/path");
            Assert.AreEqual("www.google.com", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.AreEqual("google.com", summary.Domain.RegistrableDomain);
        }

        [TestMethod]
        public void StandardLocalFile()
        {
            var summary = URLSummary.FromURL("file://c/any/path/file.ext");
            Assert.AreEqual("c/any/path/file.ext", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [TestMethod]
        public void MalformedLocalFile()
        {
            var summary = URLSummary.FromURL(@"c:\any\path\file.ext");
            Assert.AreEqual(@"c:\any\path\file.ext", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [TestMethod]
        public void StandardHttpsWithPortAndPath()
        {
            var summary = URLSummary.FromURL("http://www.google.com:12345/any/path");
            Assert.AreEqual("www.google.com:12345", summary.HostnameAndPort);
            Assert.AreEqual("12345", summary.Port);
            Assert.AreEqual("google.com", summary.Domain.RegistrableDomain);
        }

        [TestMethod]
        public void StandardData()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [TestMethod]
        public void DataEndingWithQSAndFile()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com?anything.file://");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }
        
        [TestMethod]
        public void DataEndingWithFile()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com.file://");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [TestMethod]
        public void DataEndingWithQSAndHttps()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com?anything.https://");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }
        
    }
}
