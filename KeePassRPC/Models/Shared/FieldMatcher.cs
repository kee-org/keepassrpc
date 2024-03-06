using System;

namespace KeePassRPC.Models.Shared
{
    // How we can locate a field in the client. At least one property must be set.
    // An array property matches if any of its items match.
    public class FieldMatcher : IEquatable<FieldMatcher>
    {
        public MatcherLogic? MatchLogic; // default to Client initially
        public string[] Ids; // HTML id attribute
        public string[] Names; // HTML name attribute
        public string[] Types; // HTML input type
        public string[] Queries; // HTML DOM select query
        public string[] Labels; // HTML Label or otherwise visible UI label
        public string[] AutocompleteValues; // HTML autocomplete attribute values
        public int? MaxLength; // max chars allowed in a candidate field for this to match
        public int? MinLength; // min chars allowed in a candidate field for this to match

        public bool Equals(FieldMatcher other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatchLogic == other.MatchLogic && Equals(Ids, other.Ids) && Equals(Names, other.Names) && Equals(Types, other.Types) && Equals(Queries, other.Queries) && Equals(Labels, other.Labels) && Equals(AutocompleteValues, other.AutocompleteValues) && MaxLength == other.MaxLength && MinLength == other.MinLength;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FieldMatcher)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MatchLogic.GetHashCode();
                hashCode = (hashCode * 397) ^ (Ids != null ? Ids.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Names != null ? Names.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Types != null ? Types.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Queries != null ? Queries.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Labels != null ? Labels.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AutocompleteValues != null ? AutocompleteValues.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MaxLength.GetHashCode();
                hashCode = (hashCode * 397) ^ MinLength.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(FieldMatcher left, FieldMatcher right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FieldMatcher left, FieldMatcher right)
        {
            return !Equals(left, right);
        }
    }
}