#region License, Terms and Conditions
//
// Jayrock - A JSON-RPC implementation for the Microsoft .NET Framework
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) Atif Aziz. All rights reserved.
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
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonDefaultValueAttribute
    {
        [ Test ]
        public void ByteInitialization()
        {
            AssertInitialization((byte) 42);
        }

        [ Test ]
        public void Int16Initialization()
        {
            AssertInitialization((short) 42);
        }

        [ Test ]
        public void Int32Initialization()
        {
            AssertInitialization(42);
        }

        [ Test ]
        public void Int64Initialization()
        {
            AssertInitialization(42L);
        }

        [ Test ]
        public void SingleInitialization()
        {
            AssertInitialization(42f);
        }

        [ Test ]
        public void DoubleInitialization()
        {
            AssertInitialization(42.0);
        }

        [ Test ]
        public void CharInitialization()
        {
            AssertInitialization('X');
        }

        [ Test ]
        public void StringInitialization()
        {
            AssertInitialization("foobar");
        }

        [ Test ]
        public void BooleanInitialization()
        {
            AssertInitialization(true);
            AssertInitialization(false);
        }

        [ Test ]
        public void ObjectInitialization()
        {
            AssertInitialization(new object());
        }

        [ Test ]
        public void TypedInitialization()
        {
            const string guidString = "79588595-234b-4abc-9624-d221feb2a816";
            JsonDefaultValueAttribute attribute = new JsonDefaultValueAttribute(typeof(Guid), guidString);
            Assert.IsNotNull(attribute.Value);
            Assert.IsInstanceOfType(typeof(Guid), attribute.Value);
            Assert.AreEqual(new Guid(guidString), attribute.Value);
        }

        [ Test ]
        public void PropertyDescriptorCustomizedAsJsonObjectMemberExporter()
        {
            TestPropertyDescriptor property = new TestPropertyDescriptor("prop", 42);
            JsonDefaultValueAttribute attribute = new JsonDefaultValueAttribute(42);
            IServiceProvider sp = property;
            Assert.IsNull(sp.GetService(typeof(IObjectMemberExporter)));
            IPropertyDescriptorCustomization customization = attribute;
            customization.Apply(property);
            IObjectMemberExporter exporter = (IObjectMemberExporter) sp.GetService(typeof(IObjectMemberExporter));
            Assert.IsNotNull(exporter);
        }

        [ Test ]
        public void PropertyDescriptorNotCustomizedWhenDefaultValueIsNull()
        {
            TestPropertyDescriptor property = new TestPropertyDescriptor("prop", null);
            JsonDefaultValueAttribute attribute = new JsonDefaultValueAttribute(null);
            IServiceProvider sp = property;
            IPropertyDescriptorCustomization customization = attribute;
            customization.Apply(property);
            Assert.IsNull(sp.GetService(typeof(IObjectMemberExporter)));
        }

        [ Test ]
        public void ExportsPropertyValueWhenNotEqualsSpecfiedDefault()
        {
            const string propertyName = "prop";
            IObjectMemberExporter exporter = CreatePropertyExporter(propertyName, 42, 0);

            ExportContext context = new ExportContext();
            JsonRecorder writer = new JsonRecorder();
            writer.WriteStartObject();
            exporter.Export(context, writer, new object());
            writer.WriteEndObject();
            
            JsonReader reader = writer.CreatePlayer();
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual(propertyName, reader.ReadMember());
            Assert.AreEqual(42, reader.ReadNumber().ToInt32());
        }

        [ Test ]
        public void DoesNotExportPropertyValueWhenEqualsSpecfiedDefault()
        {
            IObjectMemberExporter exporter = CreatePropertyExporter("prop", 0, 0);

            ExportContext context = new ExportContext();
            JsonRecorder writer = new JsonRecorder();
            writer.WriteStartObject();
            exporter.Export(context, writer, new object());
            writer.WriteEndObject();

            JsonReader reader = writer.CreatePlayer();
            reader.ReadToken(JsonTokenClass.Object);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void ThrowsWhenExportingWithNullContext()
        {
            CreatePropertyExporter("prop", 1, 0).Export(null, new JsonRecorder(), new object());
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void ThrowsWhenExportingWithNullWriter()
        {
            CreatePropertyExporter("prop", 1, 0).Export(new ExportContext(), null, new object());
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void ThrowsWhenExportingWithNullSource()
        {
            CreatePropertyExporter("prop", 1, 0).Export(new ExportContext(), new JsonRecorder(), null);
        }

        [ Test ]
        public void ResetValue()
        {
            JsonDefaultValueAttribute attribute = new JsonDefaultValueAttribute(42);
            attribute.Value = "foobar";
            Assert.AreEqual("foobar", attribute.Value);
        }

        private static IObjectMemberExporter CreatePropertyExporter(string name, int value, int defaultValue) 
        {
            TestPropertyDescriptor property = new TestPropertyDescriptor(name, value);
            JsonDefaultValueAttribute attribute = new JsonDefaultValueAttribute(defaultValue);
            ((IPropertyDescriptorCustomization)attribute).Apply(property);
            IServiceProvider sp = property;
            return (IObjectMemberExporter) sp.GetService(typeof(IObjectMemberExporter));
        }

        private static void AssertInitialization(object value)
        {
            JsonDefaultValueAttribute attribute = new JsonDefaultValueAttribute(value);
            Assert.IsNotNull(attribute.Value);
            Assert.IsInstanceOfType(value.GetType(), attribute.Value);
            Assert.AreEqual(value, attribute.Value);
        }

        private sealed class TestPropertyDescriptor : PropertyDescriptor, IServiceContainer
        {
            private readonly object _value;
            private readonly ServiceContainer _sc = new ServiceContainer();

            public TestPropertyDescriptor(string name, object value)
                : base(name, null)
            {
                _value = value;
            }

            public override object GetValue(object component)
            {
                return _value;
            }

            void IServiceContainer.AddService(Type serviceType, object serviceInstance)
            {
                _sc.AddService(serviceType, serviceInstance);
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                return _sc.GetService(serviceType);
            }

            #region Unimplemented members of PropertyDescriptor

            public override bool CanResetValue(object component) { throw new NotImplementedException(); }
            public override void ResetValue(object component) { throw new NotImplementedException(); }
            public override void SetValue(object component, object value) { throw new NotImplementedException(); }
            public override bool ShouldSerializeValue(object component) { throw new NotImplementedException(); }
            public override Type ComponentType { get { throw new NotImplementedException(); } }
            public override bool IsReadOnly { get { throw new NotImplementedException(); } }
            public override Type PropertyType { get { throw new NotImplementedException(); } }

            #endregion

            #region Unimplemented members of IServiceContainer

            void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) { throw new NotImplementedException(); }
            void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) { throw new NotImplementedException(); }
            void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) { throw new NotImplementedException(); }
            void IServiceContainer.RemoveService(Type serviceType) { throw new NotImplementedException(); }
            void IServiceContainer.RemoveService(Type serviceType, bool promote) { throw new NotImplementedException(); }

            #endregion
        }
    }
}
