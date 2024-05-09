namespace KeePassRPC.Models.Shared
{
    public class ResolvedField : Field
    {
        public string ResolvedValue;
        
        public bool Equals(ResolvedField other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ResolvedValue == other.ResolvedValue && Uuid.Equals(other.Uuid) && Name == other.Name && ValuePath == other.ValuePath && Value == other.Value && Page == other.Page && Type == other.Type && PlaceholderHandling == other.PlaceholderHandling && Equals(MatcherConfigs, other.MatcherConfigs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ResolvedField)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Uuid.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValuePath != null ? ValuePath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ResolvedValue != null ? ResolvedValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Page;
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ PlaceholderHandling.GetHashCode();
                hashCode = (hashCode * 397) ^ (MatcherConfigs != null ? MatcherConfigs.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ResolvedField left, ResolvedField right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ResolvedField left, ResolvedField right)
        {
            return !Equals(left, right);
        }
    }
}