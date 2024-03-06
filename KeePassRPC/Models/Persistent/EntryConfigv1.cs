using System;
using System.Collections.Generic;
using KeePassLib;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.Shared;

namespace KeePassRPC.Models.Persistent
{
    // We have no immutable properties and no known need for GetHashCode so default behaviour is as good as any
#pragma warning disable CS0659
    public class EntryConfigv1
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public int Version = 1;
        public string FormActionURL;
        public string HTTPRealm;
        public FormField[] FormFieldList;
        public bool AlwaysAutoFill;
        public bool NeverAutoFill;
        public bool AlwaysAutoSubmit;
        public bool NeverAutoSubmit;
        public int Priority;
        public string[] AltURLs;
        public bool Hide;
        public string[] BlockedURLs;
        public string[] RegExBlockedURLs;
        public string[] RegExURLs;

        /// <summary>
        /// Exact match required
        /// </summary>
        /// <remarks>This has to be public because Jayrock</remarks>
        public bool BlockHostnameOnlyMatch;

        /// <summary>
        /// Hostname/port match required
        /// </summary>
        /// <remarks>This has to be public because Jayrock</remarks>
        public bool BlockDomainOnlyMatch;

        /// <remarks>This has to be a method because Jayrock</remarks>
        public MatchAccuracyMethod? GetMatchAccuracyMethod(bool nullDefault = false)
        {
            if (BlockHostnameOnlyMatch) return MatchAccuracyMethod.Exact;
            if (BlockDomainOnlyMatch) return MatchAccuracyMethod.Hostname;
            if (nullDefault) return null;
            return MatchAccuracyMethod.Domain;
        }

        /// <remarks>This has to be a method because Jayrock</remarks>
        public void SetMatchAccuracyMethod(MatchAccuracyMethod value)
        {
            if (value == MatchAccuracyMethod.Domain)
            {
                BlockDomainOnlyMatch = false;
                BlockHostnameOnlyMatch = false;
            }
            else if (value == MatchAccuracyMethod.Hostname)
            {
                BlockDomainOnlyMatch = true;
                BlockHostnameOnlyMatch = false;
            } else
            {
                BlockDomainOnlyMatch = false;
                BlockHostnameOnlyMatch = true;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntryConfigv1"/> class.
        /// Match configuration depends on defaults in DB settings
        /// </summary>
        public EntryConfigv1(MatchAccuracyMethod accuracyMethod)
        {
            switch (accuracyMethod)
            {
                case MatchAccuracyMethod.Exact: BlockDomainOnlyMatch = false; BlockHostnameOnlyMatch = true; break;
                case MatchAccuracyMethod.Hostname: BlockDomainOnlyMatch = true; BlockHostnameOnlyMatch = false; break;
                case MatchAccuracyMethod.Domain: BlockDomainOnlyMatch = false; BlockHostnameOnlyMatch = false; break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryConfigv1"/> class.
        /// Match configuration defaults to MatchAccuracyMethod.Domain. In practice 
        /// this is only called by Jayrock deserialisation methods so the match accuracy
        /// method will be set to whatever value is stored in the JSON being used to
        /// represent this EntryConfig when at rest inside a custom string.
        /// </summary>
        public EntryConfigv1()
        {
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            EntryConfigv1 p = obj as EntryConfigv1;
            if (p == null)
                return false;

            return Version == p.Version
                   && FormActionURL == p.FormActionURL
                   && HTTPRealm == p.HTTPRealm
                   && AlwaysAutoFill == p.AlwaysAutoFill
                   && NeverAutoFill == p.NeverAutoFill
                   && AlwaysAutoSubmit == p.AlwaysAutoSubmit
                   && NeverAutoSubmit == p.NeverAutoSubmit
                   && Priority == p.Priority
                   && Hide == p.Hide
                   && BlockHostnameOnlyMatch == p.BlockHostnameOnlyMatch
                   && BlockDomainOnlyMatch == p.BlockDomainOnlyMatch
                   && AreEqual(FormFieldList, p.FormFieldList)
                   && AreEqual(AltURLs, p.AltURLs)
                   && AreEqual(BlockedURLs, p.BlockedURLs)
                   && AreEqual(RegExBlockedURLs, p.RegExBlockedURLs)
                   && AreEqual(RegExURLs, p.RegExURLs);
        }

        private bool AreEqual<T>(T[] a, T[] b)
        {
            return AreEqual(a, b, EqualityComparer<T>.Default);
        }

        private bool AreEqual<T>(T[] a, T[] b, IEqualityComparer<T> comparer)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!comparer.Equals(a[i], b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public EntryConfigv2 ConvertToV2(IGuidService guidService)
        {
            var conf2 = new EntryConfigv2();
            
            if (NeverAutoFill)
                conf2.Behaviour = EntryAutomationBehaviour.NeverAutoFillNeverAutoSubmit;
            else if (AlwaysAutoSubmit)
                conf2.Behaviour = EntryAutomationBehaviour.AlwaysAutoFillAlwaysAutoSubmit;
            else if (AlwaysAutoFill && NeverAutoSubmit)
                conf2.Behaviour = EntryAutomationBehaviour.AlwaysAutoFillNeverAutoSubmit;
            else if (NeverAutoSubmit)
                conf2.Behaviour = EntryAutomationBehaviour.NeverAutoSubmit;
            else if (AlwaysAutoFill)
                conf2.Behaviour = EntryAutomationBehaviour.AlwaysAutoFill;
            //else default (can be persisted as null)

            conf2.BlockedUrls = BlockedURLs;
            conf2.HttpRealm = string.IsNullOrEmpty(HTTPRealm) ? null : HTTPRealm;
            conf2.AltUrls = AltURLs;
            conf2.RegExUrls = RegExURLs;
            conf2.RegExBlockedUrls = RegExBlockedURLs;
            conf2.AuthenticationMethods = new[] {"password"};
            conf2.Fields = ConvertFields(FormFieldList, guidService);
            var mcList = new List<EntryMatcherConfig>
            {
                new EntryMatcherConfig {MatcherType = EntryMatcherType.Url, UrlMatchMethod = GetMatchAccuracyMethod(true)}
            };
            if (Hide)
            {
                mcList.Add(new EntryMatcherConfig {MatcherType = EntryMatcherType.Hide});
            }

            conf2.MatcherConfigs = mcList.ToArray();

            return conf2;
        }

        private Field[] ConvertFields(FormField[] formFieldList, IGuidService guidService)
        {
            var fields = new List<Field>();
            bool usernameFound = false;
            bool passwordFound = false;
            if (formFieldList != null)
            {
                foreach (var ff in formFieldList)
                {
                    if (ff.Value == "{USERNAME}")
                    {
                        usernameFound = true;
                        var mc = string.IsNullOrEmpty(ff.Id) && string.IsNullOrEmpty(ff.Name)
                            ? new FieldMatcherConfig { MatcherType = FieldMatcherType.UsernameDefaultHeuristic }
                            : FieldMatcherConfig.ForSingleClientMatch(ff.Id, ff.Name, FormFieldType.FFTusername);
                        var f = new Field
                        {
                            Page = Math.Max(ff.Page, 1),
                            ValuePath = PwDefs.UserNameField,
                            Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                            Type = FieldType.Text,
                            MatcherConfigs = new[] { mc }
                        };
                        if (ff.PlaceholderHandling == PlaceholderHandling.Default)
                        {
                            f.PlaceholderHandling = null;
                        }
                        else
                        {
                            f.PlaceholderHandling = ff.PlaceholderHandling;
                        }
                        fields.Add(f);
                    }
                    else if (ff.Value == "{PASSWORD}")
                    {
                        passwordFound = true;
                        var mc = string.IsNullOrEmpty(ff.Id) && string.IsNullOrEmpty(ff.Name)
                            ? new FieldMatcherConfig { MatcherType = FieldMatcherType.PasswordDefaultHeuristic }
                            : FieldMatcherConfig.ForSingleClientMatch(ff.Id, ff.Name, FormFieldType.FFTpassword);
                        var f = new Field
                        {
                            Page = Math.Max(ff.Page, 1),
                            ValuePath = PwDefs.PasswordField,
                            Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                            Type = FieldType.Password,
                            MatcherConfigs = new[] { mc }
                        };
                        if (ff.PlaceholderHandling == PlaceholderHandling.Default)
                        {
                            f.PlaceholderHandling = null;
                        }
                        else
                        {
                            f.PlaceholderHandling = ff.PlaceholderHandling;
                        }
                        fields.Add(f);
                    }
                    else
                    {
                        var mc = FieldMatcherConfig.ForSingleClientMatch(ff.Id, ff.Name, ff.Type);
                        var newUniqueId = Convert.ToBase64String(guidService.NewGuid().ToByteArray());
                        var f = new Field
                        {
                            Name = !string.IsNullOrWhiteSpace(ff.DisplayName) ? ff.DisplayName : newUniqueId,
                            Page = Math.Max(ff.Page, 1),
                            ValuePath = ".",
                            Uuid = newUniqueId,
                            Type = Utilities.FormFieldTypeToFieldType(ff.Type),
                            MatcherConfigs = new[] { mc },
                            Value = ff.Value
                        };
                        if (ff.PlaceholderHandling == PlaceholderHandling.Default)
                        {
                            f.PlaceholderHandling = null;
                        }
                        else
                        {
                            f.PlaceholderHandling = ff.PlaceholderHandling;
                        }
                        fields.Add(f);
                    }
                }
            }

            if (!usernameFound)
            {
                fields.Add(new Field
                {
                    ValuePath = PwDefs.UserNameField,
                    Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                    Type = FieldType.Text,
                    MatcherConfigs = new [] { new FieldMatcherConfig { MatcherType = FieldMatcherType.UsernameDefaultHeuristic } }
                });
            }
            if (!passwordFound)
            {
                fields.Add(new Field
                {
                    ValuePath = PwDefs.PasswordField,
                    Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                    Type = FieldType.Password,
                    MatcherConfigs = new [] { new FieldMatcherConfig { MatcherType = FieldMatcherType.PasswordDefaultHeuristic } }
                });
            }

            return fields.ToArray();
        }
    }
}