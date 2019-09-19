using KeePassRPC;
using DomainPublicSuffix;
using NUnit.Framework;
using System;
using KeePassLib.Utility;

namespace KeePassRPCTest
{
    [TestFixture]
    public class URLSummaryTest
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            var url = "https://publicsuffix.org/list/public_suffix_list.dat";
            //var url = "https://127.0.0.1/this/does/exist/public_suffix_list.dat";
            var cacheFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            cacheFile = UrlUtil.EnsureTerminatingSeparator(cacheFile, false);
            cacheFile = cacheFile + "publicSuffixDomainCache.txt";
            Console.WriteLine("Cache file: " + cacheFile);
            TLDRulesCache.Init(cacheFile, url);
        }

        [Test]
        public void StandardHttpWithPath()
        {
            var summary = URLSummary.FromURL("http://www.google.com/any/path");
            Assert.AreEqual("www.google.com", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.AreEqual("google.com", summary.Domain.RegistrableDomain);
        }

        [Test]
        public void StandardLocalFile()
        {
            var summary = URLSummary.FromURL("file://c/any/path/file.ext");
            Assert.AreEqual("c/any/path/file.ext", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [Test]
        public void MalformedLocalFileWithoutExtension()
        {
            var summary = URLSummary.FromURL(@"c:\any\path\file");
            Assert.AreEqual(@"c:\any\path\file", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [Test]
        public void MalformedLocalFileWithExtension()
        {
            var summary = URLSummary.FromURL(@"c:\any\path\file.ext");
            Assert.AreEqual(@"c:\any\path\file.ext", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.AreEqual("ext", summary.Domain.TLD);
        }

        [Test]
        public void StandardHttpsWithPortAndPath()
        {
            var summary = URLSummary.FromURL("http://www.google.com:12345/any/path");
            Assert.AreEqual("www.google.com:12345", summary.HostnameAndPort);
            Assert.AreEqual("12345", summary.Port);
            Assert.AreEqual("google.com", summary.Domain.RegistrableDomain);
        }

        [Test]
        public void StandardData()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [Test]
        public void DataEndingWithQSAndFile()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com?anything.file://");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }
        
        [Test]
        public void DataEndingWithFile()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com.file://");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

        [Test]
        public void DataEndingWithQSAndHttps()
        {
            var summary = URLSummary.FromURL("data:,_www.google.com?anything.https://");
            Assert.AreEqual("", summary.HostnameAndPort);
            Assert.AreEqual("", summary.Port);
            Assert.IsNull(summary.Domain);
        }

    }
}
