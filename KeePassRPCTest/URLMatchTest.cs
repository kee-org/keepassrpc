using System;
using System.Text;
using System.Collections.Generic;
using KeePassLib;
using KeePassRPC.DataExchangeModel;
using KeePassRPC;
using KeePassLib.Security;
using NUnit.Framework;
using DomainPublicSuffix;

namespace KeePassRPCTest
{
    /// <summary>
    /// Summary description for URLMatchTest
    /// </summary>
    [TestFixture]
    public class URLMatchTest
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            TLDRulesCache.Init(@"C:\temp\publicSuffixDomainCache.txt");
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestCase("https://www.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyMethod.Domain)]
        [TestCase("https://www.kee.pm", MatchAccuracyMethod.Hostname, MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyMethod.Hostname)]
        [TestCase("https://www.kee.pm", MatchAccuracyMethod.Exact, MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyMethod.Exact)]
        [TestCase("https://www.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyMethod.Domain)]
        [TestCase("https://subdom1.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Hostname, new string[] { "kee.pm" }, new MatchAccuracyMethod[] { MatchAccuracyMethod.Domain }, ExpectedResult = MatchAccuracyMethod.Domain)]
        [TestCase("https://subdom2.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Hostname, new string[] { "kee.pm" }, new MatchAccuracyMethod[] { MatchAccuracyMethod.Hostname }, ExpectedResult = MatchAccuracyMethod.Hostname)]
        [TestCase("https://www1.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Hostname, new string[] { "keeeeeee.pm" }, new MatchAccuracyMethod[] { MatchAccuracyMethod.Hostname }, ExpectedResult = MatchAccuracyMethod.Domain)]
        [TestCase("https://www1.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Hostname, new string[] { "kee.pm", "notkee.pm" }, new MatchAccuracyMethod[] { MatchAccuracyMethod.Hostname, MatchAccuracyMethod.Hostname }, ExpectedResult = MatchAccuracyMethod.Hostname)]
        [TestCase("https://www2.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Domain, new string[] { "kee.pm", "notkee.pm" }, new MatchAccuracyMethod[] { MatchAccuracyMethod.Hostname, MatchAccuracyMethod.Hostname }, ExpectedResult = MatchAccuracyMethod.Hostname)]
        [TestCase("https://www2.kee.pm", MatchAccuracyMethod.Domain, MatchAccuracyMethod.Domain, new string[] { "kee.pm", "notkee.pm" }, new MatchAccuracyMethod[] { MatchAccuracyMethod.Hostname, MatchAccuracyMethod.Domain }, ExpectedResult = MatchAccuracyMethod.Hostname)]
        public MatchAccuracyMethod SelectsCorrectMatchAccuracyMethod(
            string urlSearch, 
            MatchAccuracyMethod entryMam, 
            MatchAccuracyMethod defaultMam,
            string[] overrideURLs = null, 
            MatchAccuracyMethod[] overrideMethods = null)
        {
            var pwe = new PwEntry(true, true);
            var conf = new EntryConfig(entryMam);
            pwe.SetKPRPCConfig(conf);
            var urlSummary = URLSummary.FromURL(urlSearch);
            var dbConf = new DatabaseConfig() { DefaultMatchAccuracy = defaultMam };
            if (overrideURLs != null)
            {
                for (int i = 0; i < overrideURLs.Length; i++)
                    dbConf.MatchedURLAccuracyOverrides.Add(overrideURLs[i], overrideMethods[i]);
            }

            return pwe.GetMatchAccuracyMethod(urlSummary, dbConf);
        }

        // IPv4
        [TestCase("https://1.2.3.4:1234/path", "https://1.2.3.4:1234/path", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://1.2.3.4:1234/path", "https://1.2.3.4:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://1.2.3.4:1234/path", "https://1.2.3.4:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://1.2.3.4:1234/path", "https://1.2.3.4:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4:1234/path", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4:1234/path", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4:1234/path", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Hostname)]
        [TestCase("https://1.2.3.4:1234", "https://1.2.3.4", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]

        // IPv6
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234/path", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Hostname)]
        [TestCase("https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:1234", "https://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]


        [TestCase("https://www.kee.pm:1234/path", "https://www.kee.pm:1234/path", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://www.kee.pm:1234/path", "https://www.kee.pm:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://www.kee.pm:1234/path", "https://www.kee.pm:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://www.kee.pm:1234/path", "https://www.kee.pm:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm:1234/path", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm:1234/path", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm:1234/path", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Hostname)]
        [TestCase("https://www.kee.pm:1234", "https://www.kee.pm", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]


        [TestCase("https://www.kee.pm:1234", "https://other.kee.pm:1234", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm:1234", "https://other.kee.pm:1234", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Domain)]
        [TestCase("https://www.kee.pm:1234", "https://other.kee.pm:1234", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm:1234", "https://other.kee.pm", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm:1234", "https://other.kee.pm", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Domain)]
        [TestCase("https://www.kee.pm:1234", "https://other.kee.pm", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]


        [TestCase("https://www.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://www.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("https://www.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.Best)]
        [TestCase("http://www.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("http://www.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("http://www.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://www.kee.pm", "http://www.kee.pm", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://www.kee.pm", "http://www.kee.pm", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://www.kee.pm", "http://www.kee.pm", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://subdom.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.Domain)]
        [TestCase("https://subdom.kee.pm", "https://www.kee.pm", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.None)]

        [TestCase("http://twitter.com", "https://twitter.com", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("http://twitter.com", "https://twitter.com", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("http://twitter.com", "https://twitter.com", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]
        [TestCase("https://twitter.com", "http://twitter.com", MatchAccuracyMethod.Domain, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://twitter.com", "http://twitter.com", MatchAccuracyMethod.Hostname, ExpectedResult = MatchAccuracyEnum.HostnameAndPort)]
        [TestCase("https://twitter.com", "http://twitter.com", MatchAccuracyMethod.Exact, ExpectedResult = MatchAccuracyEnum.None)]

        public MatchAccuracyEnum CalculatesCorrectMatchAccuracyScore(string urlEntry, string urlSearch, MatchAccuracyMethod entryMam)
        {
            var pwe = new PwEntry(true, true);
            pwe.Strings.Set("URL", new ProtectedString(false, urlEntry));
            var conf = new EntryConfig(entryMam);
            pwe.SetKPRPCConfig(conf);
            var urlSummary = URLSummary.FromURL(urlSearch);

            return (MatchAccuracyEnum)KeePassRPCService.BestMatchAccuracyForAnyURL(pwe, conf, urlSearch, urlSummary, entryMam);
        }
    }
}
