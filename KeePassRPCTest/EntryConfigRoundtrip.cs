using System;
using KeePassLib;
using KeePassLib.Security;
using KeePassRPC;
using KeePassRPC.Models.Persistent;
using NUnit.Framework;

namespace KeePassRPCTest
{
    [TestFixture]
    public class EntryConfigRoundtrip
    {
        
        [TestCase("{\"version\":2,\"altUrls\":[],\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"1d3a5ab4-e430-4fd6-89f9-60e6afe3ef54\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"password\"],\"names\":[\"password\"]}}]},{\"uuid\":\"22fec3ae-0996-4b15-8cd9-6e7605bfa06a\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"username\"],\"names\":[\"username\"]}}]}]}")]
        public void PersistedJSONCorrect(
            string persistedJson)
        {
            var pwe = new PwEntry(true, true);
            pwe.CustomData.Set("KPRPC JSON", persistedJson);
            var config = pwe.GetKPRPCConfigV2(MatchAccuracyMethod.Domain);
            pwe.SetKPRPCConfig(config);
            var sut = pwe.CustomData.Get("KPRPC JSON");
            Assert.AreEqual(persistedJson, sut, "Expected: " + persistedJson + Environment.NewLine + "Actual: " + sut);
        }
    }
}