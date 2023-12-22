using System.Collections.Generic;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.Shared;

namespace KeePassRPC
{
    public class DatabaseConfig
    {
        public int Version = 3;
        public string RootUUID;
        public MatchAccuracyMethod DefaultMatchAccuracy;
        public Dictionary<string, MatchAccuracyMethod> MatchedURLAccuracyOverrides;
        public PlaceholderHandling DefaultPlaceholderHandling;

        public DatabaseConfig()
        {
            DefaultMatchAccuracy = MatchAccuracyMethod.Domain;
            MatchedURLAccuracyOverrides = new Dictionary<string, MatchAccuracyMethod>();
            DefaultPlaceholderHandling = PlaceholderHandling.Disabled;
        }
    }
}
