using System.Collections.Generic;

namespace KeePassRPC
{
    public class DatabaseConfig
    {
        public int Version = 3;
        public string RootUUID;
        public MatchAccuracyMethod DefaultMatchAccuracy;
        public Dictionary<DomainPublicSuffix.DomainName, MatchAccuracyMethod> MatchedURLAccuracyOverrides;

        public DatabaseConfig()
        {
            DefaultMatchAccuracy = MatchAccuracyMethod.Domain;
        }
    }
}
