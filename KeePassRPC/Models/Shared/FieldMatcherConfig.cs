using System;
using KeePassRPC.Models.DataExchange;

namespace KeePassRPC.Models.Shared
{
    public class FieldMatcherConfig : IEquatable<FieldMatcherConfig>
    {
        public FieldMatcherType? MatcherType;
        public FieldMatcher CustomMatcher;
        public int? Weight; // 0 = client decides or ignores locator
        public MatchAction? ActionOnMatch;

        public static FieldMatcherConfig ForSingleClientMatch(string id, string name, FormFieldType fft)
        {
            var htmlType = Utilities.FormFieldTypeToHtmlType(fft);
            return FieldMatcherConfig.ForSingleClientMatch(id, name, htmlType, null);
        }

        public static FieldMatcherConfig ForSingleClientMatch(string id, string name, string htmlType, string domSelector)
        {
            return new FieldMatcherConfig()
            {
                CustomMatcher = new FieldMatcher()
                {
                    MatchLogic = MatcherLogic.Client,
                    Ids = string.IsNullOrEmpty(id) ? new string[0] : new[] { id },
                    Names = string.IsNullOrEmpty(name) ? new string[0] : new[] { name },
                    Types = string.IsNullOrEmpty(htmlType) ? new string[0] : new []{ htmlType },
                    Queries = string.IsNullOrEmpty(domSelector) ? new string[0] : new []{ domSelector }
                }
            };
        }

        public bool Equals(FieldMatcherConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatcherType == other.MatcherType && Equals(CustomMatcher, other.CustomMatcher) && Weight == other.Weight && ActionOnMatch == other.ActionOnMatch;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FieldMatcherConfig)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MatcherType.GetHashCode();
                hashCode = (hashCode * 397) ^ (CustomMatcher != null ? CustomMatcher.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Weight.GetHashCode();
                hashCode = (hashCode * 397) ^ ActionOnMatch.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(FieldMatcherConfig left, FieldMatcherConfig right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FieldMatcherConfig left, FieldMatcherConfig right)
        {
            return !Equals(left, right);
        }
    }
}