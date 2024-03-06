using System;

namespace KeePassRPC.Models.Shared
{
    // How we can determine if an entry matches a given search context. At least one property must be set.
    // An array property matches if any of its items match.
    // Some matchers are applied in the KPRPC server when we know we could save transferring unnecessary entries
    // to the client, others are matched according to the client's implementation of the match configuration
    public class EntryMatcher : IEquatable<EntryMatcher>
    {
        public MatcherLogic? MatchLogic; // default to Client initially
        public string[] Queries; // HTML DOM select query
        public string[] PageTitles; // HTML Page title contains
        
        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntryMatcher)obj);
        }

        public bool Equals(EntryMatcher other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatchLogic == other.MatchLogic && Equals(Queries, other.Queries) && Equals(PageTitles, other.PageTitles);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MatchLogic.GetHashCode();
                hashCode = (hashCode * 397) ^ (Queries != null ? Queries.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PageTitles != null ? PageTitles.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(EntryMatcher left, EntryMatcher right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntryMatcher left, EntryMatcher right)
        {
            return !Equals(left, right);
        }
    }
}