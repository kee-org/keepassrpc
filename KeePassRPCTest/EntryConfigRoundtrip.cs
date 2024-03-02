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
        //TODO: Change UUID formats, even though we don't actually check for them here
        [TestCase(@"{""version"":2,""httpRealm"":""re"",""authenticationMethods"":[""password""],""behaviour"":""AlwaysAutoFill"",""matcherConfigs"":[{""matcherType"":""Url"",""urlMatchMethod"":""Hostname""}],""fields"":[{""uuid"":""31064359-246c-4f97-aa14-bc9ad32ec6b5"",""valuePath"":""Password"",""page"":1,""type"":""Password"",""matcherConfigs"":[{""matcherType"":""PasswordDefaultHeuristic""}]},{""uuid"":""c404defe-f4c3-47b2-9738-36f6933dad73"",""valuePath"":""UserName"",""value"":"""",""page"":1,""type"":""Text"",""matcherConfigs"":[{""customMatcher"":{""matchLogic"":""Client"",""ids"":[""e""],""names"":[""w""]}}]},{""uuid"":""2c40c9fd-9c83-4649-bdaf-907c979ca9dc"",""name"":""rrr"",""valuePath"":""."",""value"":""KEEFOX_CHECKED_FLAG_TRUE"",""page"":3,""type"":""Toggle"",""placeholderHandling"":""Disabled"",""matcherConfigs"":[{""customMatcher"":{""matchLogic"":""Client"",""ids"":[""www""],""names"":[""rrr""]}}]}]}")]
        [TestCase("{\"version\":2,\"altUrls\":[],\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"1d3a5ab4-e430-4fd6-89f9-60e6afe3ef54\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"password\"],\"names\":[\"password\"]}}]},{\"uuid\":\"22fec3ae-0996-4b15-8cd9-6e7605bfa06a\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"username\"],\"names\":[\"username\"]}}]}]}")]
        [TestCase("{\"version\":2,\"altUrls\":[\"http://test.com\"],\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"1d3a5ab4-e430-4fd6-89f9-60e6afe3ef54\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"password\"],\"names\":[\"password\"]}}]},{\"uuid\":\"22fec3ae-0996-4b15-8cd9-6e7605bfa06a\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"username\"],\"names\":[\"username\"]}}]}]}")]
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

        [TestCase("{\"version\":2,\"altUrls\":null,\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"1d3a5ab4-e430-4fd6-89f9-60e6afe3ef54\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":null,\"names\":[\"password\"]}}]},{\"uuid\":\"22fec3ae-0996-4b15-8cd9-6e7605bfa06a\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"customMatcher\":{\"names\":[\"\"]}}]}]}")]
        public void ToleratesMissingArrays(
            string persistedJson)
        {
            var pwe = new PwEntry(true, true);
            pwe.CustomData.Set("KPRPC JSON", persistedJson);
            var config = pwe.GetKPRPCConfigV2(MatchAccuracyMethod.Domain);
            pwe.SetKPRPCConfig(config);
            var sut = pwe.GetKPRPCConfigV2(MatchAccuracyMethod.Domain);
            Assert.IsNotNull(sut);
            Assert.AreEqual(0, sut.AltUrls?.Length ?? 0);
            Assert.AreEqual(0, sut.RegExBlockedUrls?.Length ?? 0);
            Assert.AreEqual(1, sut.MatcherConfigs?.Length ?? 0);
            Assert.AreEqual(2, sut.Fields?.Length ?? 0);
            Assert.AreEqual(1, sut.Fields[0].MatcherConfigs?.Length ?? 0);
            Assert.AreEqual(1, sut.Fields[1].MatcherConfigs?.Length ?? 0);
            Assert.AreEqual(0, sut.Fields[0].MatcherConfigs[0].CustomMatcher.Ids?.Length ?? 0);
            Assert.AreEqual(0, sut.Fields[1].MatcherConfigs[0].CustomMatcher.Ids?.Length ?? 0);
            Assert.AreEqual(1, sut.Fields[0].MatcherConfigs[0].CustomMatcher.Names?.Length ?? 0);
            Assert.AreEqual("", sut.Fields[1].MatcherConfigs[0].CustomMatcher.Names[0]);
            Assert.AreEqual(0, sut.Fields[0].MatcherConfigs[0].CustomMatcher.AutocompleteValues?.Length ?? 0);
            Assert.AreEqual(0, sut.Fields[1].MatcherConfigs[0].CustomMatcher.AutocompleteValues?.Length ?? 0);
        }
    }
}