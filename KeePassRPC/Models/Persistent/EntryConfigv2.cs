using System;
using KeePassRPC.Models.Shared;

namespace KeePassRPC.Models.Persistent
{
    public class EntryConfigv2 : IEquatable<EntryConfigv2>
    {
        public int Version = 2;
        public string[] AltUrls;
        public string[] BlockedUrls;
        public string[] RegExBlockedUrls;
        public string[] RegExUrls;
        public string HttpRealm;
        public string[] AuthenticationMethods; // e.g. "facebook","password","passkey"
        public EntryAutomationBehaviour? Behaviour;
        public EntryMatcherConfig[] MatcherConfigs;
        public Field[] Fields;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryConfigv2"/> class.
        /// Url Match accuracy configuration depends on defaults in DB settings
        /// </summary>
        public EntryConfigv2(MatchAccuracyMethod accuracyMethod)
        {
            throw new NotImplementedException();
            //TODO: implement once we no longer go via v1 constructor and normaliser for new configs
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryConfigv2"/> class.
        /// Match configuration defaults to MatchAccuracyMethod.Domain. In practice 
        /// this is only called by Jayrock deserialisation methods so the match accuracy
        /// method will be set to whatever value is stored in the JSON being used to
        /// represent this EntryConfig when at rest inside a custom string.
        /// </summary>
        public EntryConfigv2()
        {
        }

        public bool Equals(EntryConfigv2 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Version == other.Version && Equals(AltUrls, other.AltUrls) && Equals(BlockedUrls, other.BlockedUrls) && Equals(RegExBlockedUrls, other.RegExBlockedUrls) && Equals(RegExUrls, other.RegExUrls) && HttpRealm == other.HttpRealm && Equals(AuthenticationMethods, other.AuthenticationMethods) && Behaviour == other.Behaviour && Equals(MatcherConfigs, other.MatcherConfigs) && Equals(Fields, other.Fields);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntryConfigv2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Version;
                hashCode = (hashCode * 397) ^ (AltUrls != null ? AltUrls.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BlockedUrls != null ? BlockedUrls.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (RegExBlockedUrls != null ? RegExBlockedUrls.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (RegExUrls != null ? RegExUrls.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HttpRealm != null ? HttpRealm.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AuthenticationMethods != null ? AuthenticationMethods.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Behaviour.GetHashCode();
                hashCode = (hashCode * 397) ^ (MatcherConfigs != null ? MatcherConfigs.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Fields != null ? Fields.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(EntryConfigv2 left, EntryConfigv2 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntryConfigv2 left, EntryConfigv2 right)
        {
            return !Equals(left, right);
        }
    }
}