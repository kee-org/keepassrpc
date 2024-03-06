using System;

namespace KeePassRPC.Models.Shared
{
    public class EntryMatcherConfig : IEquatable<EntryMatcherConfig>
    {
        public EntryMatcherType? MatcherType;
        public EntryMatcher CustomMatcher;
        public MatchAccuracyMethod? UrlMatchMethod;
        public int? Weight; // 0 = client decides or ignores matcher
        public MatchAction? ActionOnMatch;
        public MatchAction? ActionOnNoMatch; // critical to use TotalBlock here for Url match type
        
        public bool Equals(EntryMatcherConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatcherType == other.MatcherType && Equals(CustomMatcher, other.CustomMatcher) && UrlMatchMethod == other.UrlMatchMethod && Weight == other.Weight && ActionOnMatch == other.ActionOnMatch && ActionOnNoMatch == other.ActionOnNoMatch;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntryMatcherConfig)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MatcherType.GetHashCode();
                hashCode = (hashCode * 397) ^ (CustomMatcher != null ? CustomMatcher.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ UrlMatchMethod.GetHashCode();
                hashCode = (hashCode * 397) ^ Weight.GetHashCode();
                hashCode = (hashCode * 397) ^ ActionOnMatch.GetHashCode();
                hashCode = (hashCode * 397) ^ ActionOnNoMatch.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(EntryMatcherConfig left, EntryMatcherConfig right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntryMatcherConfig left, EntryMatcherConfig right)
        {
            return !Equals(left, right);
        }
    }
}