namespace JsonConversionsDemo
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Jayrock.Json.Conversion;
    using Jayrock.Json.Conversion.Converters;
    using CustomTypeDescriptor = Jayrock.Json.Conversion.CustomTypeDescriptor;

    #endregion

    /// <summary>
    /// DSL-ish facade for building custom types and registering them for 
    /// importing and exporting JSON Data. It makes <see cref="CustomTypeDescriptor"/>
    /// construction a little more palatable.
    /// </summary>

    public static class JsonType
    {
        public static Builder BuildFor(Type type)
        {
            return new Builder(type);
        }

        [ Serializable ]
        public sealed class Builder
        {
            private readonly Type _type;
            private readonly List<MemberInfo> _members;
            private readonly List<string> _names;
            private CustomTypeDescriptor _customType;

            public Builder(Type type)
            {
                if (type == null) throw new ArgumentNullException("type");

                _type = type;
                _members = new List<MemberInfo>();
                _names = new List<string>();
            }

            public Builder AddProperty(string name)
            {
                return AddProperty(name, null);
            }

            public Builder AddProperty(string name, string publicName)
            {
                OnChanging();
                _members.Add(_type.GetProperty(name));
                _names.Add(publicName);
                return this;
            }

            public Builder AddField(string name)
            {
                return AddField(name, null);
            }

            public Builder AddField(string name, string publicName)
            {
                OnChanging();
                _members.Add(_type.GetField(name));
                _names.Add(publicName);
                return this;
            }

            public Builder As(string name)
            {
                if (_names.Count == 0)
                    throw new InvalidOperationException("Nothing to rename.");

                OnChanging();
                _names[_names.Count - 1] = name;
                return this;
            }

            public ICustomTypeDescriptor ToCustomType()
            {
                if (_customType == null)
                    _customType = new CustomTypeDescriptor(_type, _members.ToArray(), _names.ToArray());
                
                return _customType;
            }

            public Builder Register(ExportContext context)
            {
                if (context == null) throw new ArgumentNullException("context");

                context.Register(new ComponentExporter(_type, ToCustomType()));
                return this;
            }

            public Builder Register(ImportContext context)
            {
                if (context == null) throw new ArgumentNullException("context");

                context.Register(new ComponentImporter(_type, ToCustomType()));
                return this;
            }

            private void OnChanging()
            {
                _customType = null;
            }
        }
    }
}