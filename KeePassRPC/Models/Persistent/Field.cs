using System;
using KeePassLib;
using KeePassRPC.Models.Shared;

namespace KeePassRPC.Models.Persistent
{
    public class Field : IEquatable<Field>
    {
        public Guid Uuid;
        public string Name; // display name, not form field name attribute
        public string ValuePath; // e.g. "Username" for a KeePass Property or "." for this object
        public string Value;
        public int Page = 1; // Fields with multiple positive page numbers are effectively treated as multiple Entries when Kee assesses potential matches and field candidates to fill. Other clients might use for similar logical grouping purposes.
        public FieldType @Type;
        public PlaceholderHandling? PlaceholderHandling;
        public FieldMatcherConfig[] MatcherConfigs;

        public bool Equals(Field other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Uuid.Equals(other.Uuid) && Name == other.Name && ValuePath == other.ValuePath && Value == other.Value && Page == other.Page && Type == other.Type && PlaceholderHandling == other.PlaceholderHandling && Equals(MatcherConfigs, other.MatcherConfigs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Field)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Uuid.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValuePath != null ? ValuePath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Page;
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ PlaceholderHandling.GetHashCode();
                hashCode = (hashCode * 397) ^ (MatcherConfigs != null ? MatcherConfigs.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Field left, Field right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Field left, Field right)
        {
            return !Equals(left, right);
        }
    }
}