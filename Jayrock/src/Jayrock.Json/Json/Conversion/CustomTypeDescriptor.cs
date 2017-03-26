#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    
    //
    // Types from System.ComponentModel must be imported explicitly because
    // .NET Framework 2.0 also contains a CustomTypeDescriptor in 
    // System.ComponentModel.
    //
    
    using ICustomTypeDescriptor = System.ComponentModel.ICustomTypeDescriptor;
    using PropertyDescriptorCollection = System.ComponentModel.PropertyDescriptorCollection;
    using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
    using AttributeCollection= System.ComponentModel.AttributeCollection;
    using TypeConverter = System.ComponentModel.TypeConverter;
    using EventDescriptor = System.ComponentModel.EventDescriptor;
    using EventDescriptorCollection = System.ComponentModel.EventDescriptorCollection;
    
    #endregion
    
    /// <summary>
    /// Provides an <see cref="ICustomTypeDescriptor"/> implementation on top of the
    /// public read/write fields and properties of a given type.
    /// </summary>

    public sealed class CustomTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly PropertyDescriptorCollection _properties;

        public CustomTypeDescriptor(Type type) : 
            this(type, null) {}

        public CustomTypeDescriptor(Type type, MemberInfo[] members) :
            this(type, members, null) {}

        public CustomTypeDescriptor(Type type, MemberInfo[] members, string[] names) :
            this(type, LikeAnonymousClass(type), members, names) {}

        private CustomTypeDescriptor(Type type, bool isAnonymousClass, MemberInfo[] members, string[] names)
        {
            if (type == null) 
                throw new ArgumentNullException("type");

            // TODO Remove dependency on JsonIgnore & JsonExport
            // This class should not have any JSON specifics.

            //
            // No members supplied? Get all public, instance-level fields and 
            // properties of the type that are not marked with the JsonIgnore
            // attribute.
            //
            
            if (members == null)
            {
                const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public;
                FieldInfo[] fields = type.GetFields(bindings);
                PropertyInfo[] properties = type.GetProperties(bindings);
                
                //
                // Filter out members marked with JsonIgnore attribute.
                //
                
                ArrayList memberList = new ArrayList(fields.Length + properties.Length);
                memberList.AddRange(fields);
                memberList.AddRange(properties);

                for (int i = 0; i < memberList.Count; i++)
                {
                    MemberInfo member = (MemberInfo) memberList[i];
                    
                    if (!member.IsDefined(typeof(JsonIgnoreAttribute), true))
                        continue;
                    
                    memberList.RemoveAt(i--);
                }
                
                members = (MemberInfo[]) memberList.ToArray(typeof(MemberInfo));
            }
                        
            PropertyDescriptorCollection logicalProperties = new PropertyDescriptorCollection(null);
            bool immutable = true;
            int index = 0;
            
            foreach (MemberInfo member in members)
            {
                FieldInfo field = member as FieldInfo;
                string name = names != null && index < names.Length ? names[index] : null;
                TypeMemberDescriptor descriptor = null;
                bool writable;

                if (field != null)
                {
                    //
                    // Add public fields that are not read-only and not 
                    // constant literals.
                    //
            
                    if (field.DeclaringType != type && field.ReflectedType != type)
                        throw new ArgumentException(null, "members");

                    writable = !field.IsInitOnly;
                    if ((writable || immutable) && !field.IsLiteral)
                    {
                        descriptor = new TypeFieldDescriptor(field, name);
                    }
                }
                else
                {
                    PropertyInfo property = member as PropertyInfo;
                    
                    if (property == null)
                        throw new ArgumentException(null, "members");

                    //
                    // Add public properties that can be read and modified.
                    // If property is read-only yet has the JsonExport 
                    // attribute applied then include it anyhow (assuming
                    // that the type author probably has customizations
                    // that know how to deal with the sceanrio more 
                    // accurately). What's more, if the type is anonymous 
                    // then the rule that the proerty must be writeable is
                    // also bypassed.
                    //

                    if (property.DeclaringType != type && property.ReflectedType != type)
                        throw new ArgumentException(null, "members");

                    writable = property.CanWrite;

                    if ((property.CanRead) &&
                        (writable || immutable || property.IsDefined(typeof(JsonExportAttribute), true)) &&
                        property.GetIndexParameters().Length == 0)
                    {
                        //
                        // Properties of an anonymous class will always use 
                        // their original property name so that no 
                        // transformation (like auto camel-casing) is 
                        // applied. The rationale for the exception here is
                        // that since the user does not have a chance to
                        // decorate properties of an anonymous class with
                        // attributes, there is no way an overriding policy
                        // can be implemented.
                        //

                        descriptor = new TypePropertyDescriptor(property, 
                            isAnonymousClass ? Mask.EmptyString(name, property.Name) : name);
                    }
                }
                
                if (descriptor != null)
                {
                    descriptor.ApplyCustomizations();

                    if (immutable && writable)
                    {
                        immutable = false;
                        logicalProperties.Clear();
                    }

                    logicalProperties.Add(descriptor);
                }
                
                index++;
            }
                
            _properties = logicalProperties;
        }

        public static CustomTypeDescriptor TryCreateForAnonymousClass(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            bool like = LikeAnonymousClass(type);
            return like ? new CustomTypeDescriptor(type, like, null, null) : null;
        }

        public static PropertyDescriptor CreateProperty(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            
            return new TypeFieldDescriptor(field, field.Name);
        }
            
        public static PropertyDescriptor CreateProperty(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            
            return new TypePropertyDescriptor(property, property.Name);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return _properties;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException(); // FIXME: Incomplete implementation!
        }

        /// <summary>
        /// Forward-compatible way to see if the given type is an anonymous 
        /// class (introduced since C# 3.0). 
        /// </summary>
        /// <remarks>
        /// There is no sure shot method so we have rely to rely on a 
        /// heuristic approach by looking for a few known characteristics.
        /// Note also that we take a "duck" approach to look for the 
        /// CompilerGenerated attribute under .NET Framework 1.x, which does 
        /// not seem like an appaling idea considering that the C# compiler 
        /// does the same with ExtensionAttribute when it comes to extension 
        /// methods.
        /// </remarks>
        ///

        internal static bool LikeAnonymousClass(Type type)
        {
            Debug.Assert(type != null);

            return type.IsNotPublic && type.IsClass && type.IsSealed
                && type.GetConstructor(Type.EmptyTypes) == null
#if NET_1_0 || NET_1_1
                && AnyObjectByTypeName(type.GetCustomAttributes(false),
                       "System.Runtime.CompilerServices.CompilerGeneratedAttribute")
#else
                && type.IsGenericType
                && type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
#endif
                ;
        }

#if NET_1_0 || NET_1_1

        private static bool AnyObjectByTypeName(object[] objects, string typeNameSought)
        {
            return FindFirstObjectByTypeName(objects, typeNameSought) != null;
        }

        private static object FindFirstObjectByTypeName(object[] objects, string typeNameSought)
        {
            Debug.Assert(objects != null);
            Debug.Assert(typeNameSought != null);
            Debug.Assert(typeNameSought.Length > 0);

            foreach (object obj in objects)
            {
                if (obj != null && 0 == string.CompareOrdinal(obj.GetType().FullName, typeNameSought))
                    return obj;
            }

            return null;
        }

#endif

        #region Uninteresting implementations of ICustomTypeDescriptor members

        public AttributeCollection GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        public string GetClassName()
        {
            return null;
        }

        public string GetComponentName()
        {
            return null;
        }

        public TypeConverter GetConverter()
        {
            return new TypeConverter();
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// A base <see cref="PropertyDescriptor"/> implementation for
        /// a type member (<see cref="MemberInfo"/>).
        /// </summary>

        private abstract class TypeMemberDescriptor : PropertyDescriptor, IPropertyImpl, IPropertyCustomization, IServiceContainer
        {
            private string _customName;
            private int _customNameHashCode;
            private Type _propertyType;
            private IPropertyImpl _impl;
            private ServiceContainer _services;
            
            protected TypeMemberDescriptor(MemberInfo member, string name , Type propertyType) : 
                base(ChooseName(name, member.Name), null)
            {
                Debug.Assert(propertyType != null);
                
                _impl = this;
                _propertyType = propertyType;
            }

            protected abstract MemberInfo Member { get; }
                
            public override bool Equals(object obj)
            {
                TypeMemberDescriptor other = obj as TypeMemberDescriptor;
                return other != null && other.Member.Equals(Member);
            }
            
            public override int GetHashCode() { return Member.GetHashCode(); }
            public override void ResetValue(object component) {}
            public override bool CanResetValue(object component) { return false; }
            public override bool ShouldSerializeValue(object component) { return true; }
            public override Type ComponentType { get { return Member.DeclaringType; } }
                
            public override Type PropertyType 
            { 
                get { return _propertyType; }
            }

            public override object GetValue(object component)
            {
                return _impl.GetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                if (IsReadOnly)
                    throw new NotSupportedException();
                
                _impl.SetValue(component, value);
            }

            public override string Name
            {
                get { return _customName != null ? _customName : base.Name; }
            }

            protected override int NameHashCode
            {
                get { return _customName != null ? _customNameHashCode : base.NameHashCode; }
            }

            protected abstract object GetValueImpl(object component);
            protected abstract void SetValueImpl(object component, object value);

            object IPropertyImpl.GetValue(object obj)
            {
                return GetValueImpl(obj);
            }

            void IPropertyImpl.SetValue(object obj, object value)
            {
                SetValueImpl(obj, value);
            }

            void IPropertyCustomization.SetName(string name)
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name.Length == 0)
                    throw new ArgumentException(null, "name");

                _customName = name;
                _customNameHashCode = name.GetHashCode();
            }

            void IPropertyCustomization.SetType(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");
                
                _propertyType = type;
            }

            IPropertyImpl IPropertyCustomization.OverrideImpl(IPropertyImpl impl)
            {
                if (impl == null)
                    throw new ArgumentNullException("impl");
                
                IPropertyImpl baseImpl = _impl;
                _impl = impl;
                return baseImpl;
            }

            internal void ApplyCustomizations()
            {
                IPropertyDescriptorCustomization[] customizations = (IPropertyDescriptorCustomization[]) Member.GetCustomAttributes(typeof(IPropertyDescriptorCustomization), true);
                
                if (customizations == null)
                    return;

                foreach (IPropertyDescriptorCustomization customization in customizations)
                    customization.Apply(this);
            }

            private static string ChooseName(string propsedName, string baseName)
            {
                if (Mask.NullString(propsedName).Length > 0)
                    return propsedName;
                
                return ToCamelCase(baseName);
            }

            private static string ToCamelCase(string s)
            {
                if (s == null || s.Length == 0)
                    return s;
                
                return char.ToLower(s[0], CultureInfo.InvariantCulture) + s.Substring(1);
            }

            private ServiceContainer Services
            {
                get
                {
                    if (_services == null)
                        _services = new ServiceContainer();
                    
                    return _services;
                }
            }

            #region IServiceContainer implementation

            void IServiceContainer.AddService(Type serviceType, object serviceInstance)
            {
                Services.AddService(serviceType, serviceInstance);
            }

            void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
            {
                Services.AddService(serviceType, serviceInstance, promote);
            }

            void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
            {
                Services.AddService(serviceType, callback);
            }

            void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
            {
                Services.AddService(serviceType, callback, promote);
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                return _services != null ? _services.GetService(serviceType) : null;
            }

            void IServiceContainer.RemoveService(Type serviceType)
            {
                if (_services != null)
                    _services.RemoveService(serviceType);
            }

            void IServiceContainer.RemoveService(Type serviceType, bool promote)
            {
                if (_services != null)
                    _services.RemoveService(serviceType, promote);
            }

            #endregion
        }

        /// <summary>
        /// A <see cref="PropertyDescriptor"/> implementation around
        /// <see cref="FieldInfo"/>.
        /// </summary>

        private sealed class TypeFieldDescriptor : TypeMemberDescriptor
        {
            private readonly FieldInfo _field;

            public TypeFieldDescriptor(FieldInfo field, string name) : 
                base(field, name, field.FieldType)
            {
                _field = field;
            }

            protected override MemberInfo Member
            {
                get { return _field; }
            }

            public override bool IsReadOnly
            {
                get { return _field.IsInitOnly; }
            }

            protected override object GetValueImpl(object component)
            {
                return _field.GetValue(component);
            }

            protected override void SetValueImpl(object component, object value) 
            {
                _field.SetValue(component, value); 
                OnValueChanged(component, EventArgs.Empty);
            }
        }
            
        /// <summary>
        /// A <see cref="PropertyDescriptor"/> implementation around
        /// <see cref="PropertyInfo"/>.
        /// </summary>

        private sealed class TypePropertyDescriptor : TypeMemberDescriptor
        {
            private readonly PropertyInfo _property;

            public TypePropertyDescriptor(PropertyInfo property, string name) : 
                base(property, name, property.PropertyType)
            {
                _property = property;
            }

            protected override MemberInfo Member
            {
                get { return _property; }
            }

            public override bool IsReadOnly
            {
                get { return !_property.CanWrite; }
            }

            protected override object GetValueImpl(object component)
            {
                return _property.GetValue(component, null);
            }

            protected override void SetValueImpl(object component, object value) 
            {
                _property.SetValue(component, value, null); 
                OnValueChanged(component, EventArgs.Empty);
            }
        }
    }
}
