using System;
using KeePassLib;
using KeePassLib.Security;
using KeePassRPC;
using KeePassRPC.Models.Persistent;
using NUnit.Framework;

namespace KeePassRPCTest
{
    [TestFixture]
    public class EntryConfigConvert
    {
        
        [TestCase("{\"version\":1,\"hTTPRealm\":\"\",\"formFieldList\":[{\"name\":\"password\",\"displayName\":\"KeePass password\",\"value\":\"{PASSWORD}\",\"type\":\"FFTpassword\",\"id\":\"password\",\"page\":-1,\"placeholderHandling\":\"Default\"},{\"name\":\"username\",\"displayName\":\"KeePass username\",\"value\":\"{USERNAME}\",\"type\":\"FFTradio\",\"id\":\"username\",\"page\":-1,\"placeholderHandling\":\"Default\"}],\"alwaysAutoFill\":false,\"neverAutoFill\":false,\"alwaysAutoSubmit\":false,\"neverAutoSubmit\":false,\"priority\":0,\"altURLs\":[],\"hide\":false,\"blockHostnameOnlyMatch\":false,\"blockDomainOnlyMatch\":false}", "{\"version\":2,\"altUrls\":[],\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"00000000-0000-0000-0000-000000000000\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"password\"],\"names\":[\"password\"]}}]},{\"uuid\":\"00000000-0000-0000-0000-000000000000\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"username\"],\"names\":[\"username\"]}}]}]}")]
        [TestCase("{\"version\":1,\"hTTPRealm\":\"\",\"formFieldList\":[{\"name\":\"password\",\"displayName\":\"KeePass password\",\"value\":\"{PASSWORD}\",\"type\":\"FFTpassword\",\"id\":\"password\",\"page\":-1,\"placeholderHandling\":\"Default\"},{\"name\":\"username\",\"displayName\":\"KeePass username\",\"value\":\"{USERNAME}\",\"type\":\"FFTusername\",\"id\":\"username\",\"page\":-1,\"placeholderHandling\":\"Default\"}],\"alwaysAutoFill\":false,\"neverAutoFill\":false,\"alwaysAutoSubmit\":false,\"neverAutoSubmit\":false,\"priority\":0,\"altURLs\":[],\"hide\":false,\"blockHostnameOnlyMatch\":false,\"blockDomainOnlyMatch\":false}", "{\"version\":2,\"altUrls\":[],\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"00000000-0000-0000-0000-000000000000\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"password\"],\"names\":[\"password\"]}}]},{\"uuid\":\"00000000-0000-0000-0000-000000000000\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"customMatcher\":{\"ids\":[\"username\"],\"names\":[\"username\"]}}]}]}")]
        [TestCase("","{\"version\":2,\"authenticationMethods\":[\"password\"],\"matcherConfigs\":[{\"matcherType\":\"Url\"}],\"fields\":[{\"uuid\":\"00000000-0000-0000-0000-000000000000\",\"valuePath\":\"UserName\",\"page\":1,\"type\":\"Text\",\"matcherConfigs\":[{\"matcherType\":\"UsernameDefaultHeuristic\"}]},{\"uuid\":\"00000000-0000-0000-0000-000000000000\",\"valuePath\":\"Password\",\"page\":1,\"type\":\"Password\",\"matcherConfigs\":[{\"matcherType\":\"PasswordDefaultHeuristic\"}]}]}")]
        public void PersistedJSONCorrect(
            string persistedV1, string expected)
        {
            var pwe = new PwEntry(true, true);
            pwe.Strings.Set("KPRPC JSON", new ProtectedString(
                true, persistedV1));
            var configV1 = pwe.GetKPRPCConfigV1(MatchAccuracyMethod.Domain);
            var configV2 = configV1.ConvertToV2(new MockGuidService());
            pwe.SetKPRPCConfig(configV2);
            var sut = pwe.CustomData.Get("KPRPC JSON");
            Assert.AreEqual(expected, sut, "Expected: " + expected + Environment.NewLine + "Actual: " + sut);
        }
        
        public class MockGuidService : IGuidService
        {
            public Guid NewGuid()
            {
                return Guid.Empty;
            }
        }
    }
}