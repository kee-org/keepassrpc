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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;
    using CustomTypeDescriptor = Conversion.CustomTypeDescriptor;

    #endregion

    [ TestFixture ]
    public class TestComponentExporter
    {
        [ Test ]
        public void EmptyObject()
        {
            Assert.AreEqual("[\"System.Object\"]", Format(new object()));
        }

        [ Test ]
        public void PublicProperties()
        {
            Car car = new Car();            
            car.Manufacturer = "BMW";
            car.Model = "350";
            car.Year = 2000;
            car.Color = "Silver";

            Test(new JsonObject(
                new string[] { "manufacturer", "model", "year", "color" },
                new object[] { car.Manufacturer, car.Model, car.Year, car.Color }), car);
        }

        [ Test ]
        public void NullPropertiesSkipped()
        {
            Car car = new Car();

            JsonReader reader = FormatForReading(car);
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("year", reader.ReadMember());
            Assert.AreEqual(0, (int) reader.ReadNumber());
            Assert.AreEqual(JsonTokenClass.EndObject, reader.TokenClass);
        }

        [ Test ]
        public void EmbeddedObjects()
        {
            Person snow = new Person();
            snow.Id = 2;
            snow.FullName = "Snow White";

            Person albert = new Person();
            albert.Id = 1;
            albert.FullName = "Albert White";
            
            Marriage m = new Marriage();
            m.Husband = albert;
            m.Wife = snow;

            Test(new JsonObject(
                new string[] { "husband", "wife" },
                new object[] {
                    /* Husband */ new JsonObject(
                        new string[] { "id", "fullName" },
                        new object[] { albert.Id, albert.FullName }),
                    /* Wife */ new JsonObject(
                        new string[] { "id", "fullName" },
                        new object[] { snow.Id, snow.FullName })
                }), m);
        }

        [ Test ]
        public void CustomPropertiesInternally()
        {
            Point point = new Point(123, 456);
            JsonRecorder writer = new JsonRecorder();
            JsonConvert.Export(point, writer);
            JsonReader reader = writer.CreatePlayer();
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("x", reader.ReadMember());
            Assert.AreEqual(123, reader.ReadNumber().ToInt32());
            Assert.AreEqual("y", reader.ReadMember());
            Assert.AreEqual(456, reader.ReadNumber().ToInt32());
            Assert.AreSame(JsonTokenClass.EndObject, reader.TokenClass);
            Assert.IsFalse(reader.Read());
        }

        [ Test ]
        public void TypeSpecific()
        {
            Person john = new Person();
            john.Id = 123;
            john.FullName = "John Doe";
            
            Car beamer = new Car();            
            beamer.Manufacturer = "BMW";
            beamer.Model = "350";
            beamer.Year = 2000;
            beamer.Color = "Silver";

            OwnerCars johnCars = new OwnerCars();
            johnCars.Owner = john;
            johnCars.Cars.Add(beamer);

            JsonObject test = new JsonObject(
                new string[] { "owner", "cars" }, 
                new object[] {
                    /* Owner */ new JsonObject(
                        new string[] { "id", "fullName" }, 
                        new object[] { john.Id,  john.FullName }),
                    /* Cars */ new object[] {
                        new JsonObject(
                            new string[] { "manufacturer", "model", "year", "color" }, 
                            new object[] { beamer.Manufacturer, beamer.Model, beamer.Year, beamer.Color })
                    }
                });

            Test(test, johnCars);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void ImmediateCircularReferenceDetection()
        {
            ExportContext context = new ExportContext();
            ComponentExporter exporter = new ComponentExporter(typeof(Thing));
            context.Register(exporter);
            Thing thing = new Thing();
            thing.Other = thing;
            exporter.Export(context, thing, new EmptyJsonWriter());
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void DeepCircularReferenceDetection()
        {
            ExportContext context = new ExportContext();
            ComponentExporter exporter = new ComponentExporter(typeof(Thing));
            context.Register(exporter);
            Thing thing = new Thing();
            thing.Other = new Thing();
            thing.Other.Other = new Thing();
            thing.Other.Other.Other = thing;
            exporter.Export(context, thing, new EmptyJsonWriter());
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CircularReferenceDetectionAcrossTypes()
        {
            ExportContext context = new ExportContext();
            ComponentExporter exporter = new ComponentExporter(typeof(Parent));
            context.Register(exporter);
            context.Register(new ComponentExporter(typeof(ParentChild)));
            Parent parent = new Parent();
            parent.Child = new ParentChild();
            parent.Child.Parent = parent;
            exporter.Export(context, parent, new EmptyJsonWriter());
        }
        
        [ Test ]
        public void MemberExportCustomization()
        {
            ArrayList calls = new ArrayList();

            TestTypeDescriptor logicalType = new TestTypeDescriptor();
            PropertyDescriptorCollection properties = logicalType.GetProperties();

            Hashtable services;

            TestObjectMemberExporter memexp1 = new TestObjectMemberExporter(calls);
            services = new Hashtable();
            services.Add(typeof(IObjectMemberExporter), memexp1);
            properties.Add(new TestPropertyDescriptor("prop1", services));

            TestObjectMemberExporter memexp2 = new TestObjectMemberExporter(calls);
            services = new Hashtable();
            services.Add(typeof(IObjectMemberExporter), memexp2);
            properties.Add(new TestPropertyDescriptor("prop2", services));

            ComponentExporter exporter = new ComponentExporter(typeof(Thing), logicalType);
            ExportContext context = new ExportContext();
            context.Register(exporter);
            
            JsonRecorder writer = new JsonRecorder();
            Thing thing = new Thing();
            context.Export(thing, writer);

            Assert.AreEqual(2, calls.Count);

            object[] args = { context, writer, thing };

            Assert.AreSame(memexp1, calls[0]);
            Assert.AreEqual(args, ((TestObjectMemberExporter) calls[0]).ExportArgs);

            Assert.AreSame(memexp2, calls[1]);
            Assert.AreEqual(args, ((TestObjectMemberExporter) calls[1]).ExportArgs);
        }

        private sealed class TestObjectMemberExporter : IObjectMemberExporter
        {
            public object[] ExportArgs;

            private readonly IList _sequence;
            
            public TestObjectMemberExporter(IList recorder)
            {
                _sequence = recorder;
            }

            void IObjectMemberExporter.Export(ExportContext context, JsonWriter writer, object source)
            {
                ExportArgs = new object[] { context, writer, source };
                _sequence.Add(this);
            }
        }

        private sealed class TestPropertyDescriptor : PropertyDescriptor, IServiceProvider
        {
            private readonly IDictionary _services;

            public TestPropertyDescriptor(string name, IDictionary services) : base(name, null)
            {
                _services = services;
            }
            
            object IServiceProvider.GetService(Type serviceType)
            {
                return _services[serviceType];
            }

            #region Unimplemented members of PropertyDescriptor

            public override bool CanResetValue(object component) { throw new NotImplementedException(); }
            public override object GetValue(object component) { throw new NotImplementedException(); }
            public override void ResetValue(object component) { throw new NotImplementedException(); }
            public override void SetValue(object component, object value) { throw new NotImplementedException(); }
            public override bool ShouldSerializeValue(object component) { throw new NotImplementedException(); }
            public override Type ComponentType { get { throw new NotImplementedException(); } }
            public override bool IsReadOnly { get { throw new NotImplementedException(); } }
            public override Type PropertyType { get { throw new NotImplementedException(); } }
            
            #endregion
        }

        private sealed class TestTypeDescriptor : ICustomTypeDescriptor
        {
            private readonly PropertyDescriptorCollection _properties = new PropertyDescriptorCollection(null);

            public PropertyDescriptorCollection GetProperties()
            {
                return _properties;
            }

            #region Unimplemented members of ICustomTypeDescriptor

            public AttributeCollection GetAttributes() { throw new NotImplementedException(); }
            public string GetClassName() { throw new NotImplementedException(); }
            public string GetComponentName() { throw new NotImplementedException(); }
            public TypeConverter GetConverter() { throw new NotImplementedException(); }
            public EventDescriptor GetDefaultEvent() { throw new NotImplementedException(); }
            public PropertyDescriptor GetDefaultProperty() { throw new NotImplementedException(); }
            public object GetEditor(Type editorBaseType) { throw new NotImplementedException(); }
            public EventDescriptorCollection GetEvents() { throw new NotImplementedException(); }
            public EventDescriptorCollection GetEvents(Attribute[] attributes) { throw new NotImplementedException(); }
            public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { throw new NotImplementedException(); }
            public object GetPropertyOwner(PropertyDescriptor pd) { throw new NotImplementedException(); }

            #endregion
        }

        private sealed class Thing
        {
            public Thing Other;
        }
        
        private sealed class Parent
        {
            public ParentChild Child;
        }
        
        private sealed class ParentChild
        {
            public Parent Parent;
        }

        private static string Format(object o)
        {
            JsonTextWriter writer = new JsonTextWriter();
            JsonConvert.Export(o, writer);
            return writer.ToString();
        }

        private static JsonReader FormatForReading(object o)
        {
            return new JsonTextReader(new StringReader(Format(o)));
        }

        private static void Test(JsonObject expected, object actual)
        {
            JsonReader reader = FormatForReading(actual);
            TestObject(expected, reader, "(root)");
            Assert.IsFalse(reader.Read(), "Expected EOF.");
        }

        private static void TestObject(JsonObject expected, JsonReader reader, string path)
        {
            reader.MoveToContent();
            reader.ReadToken(JsonTokenClass.Object);
            
            while (reader.TokenClass != JsonTokenClass.EndObject)
            {
                string name = reader.ReadMember();
                object value = expected[name];
                expected.Remove(name);
                TestValue(value, reader, path + "/" + name);
            }
            
            Assert.AreEqual(0, expected.Count);
            reader.Read();
        }

        private static void TestArray(Array expectations, JsonReader reader, string path)
        {
            reader.MoveToContent();
            reader.ReadToken(JsonTokenClass.Array);

            for (int i = 0; i < expectations.Length; i++)
                TestValue(expectations.GetValue(i), reader, path + "/" + i);

            reader.ReadToken(JsonTokenClass.EndArray);
        }

        private static void TestValue(object expected, JsonReader reader, string path)
        {
            if (JsonNull.LogicallyEquals(expected))
            {
                Assert.AreEqual(JsonTokenClass.Null, reader.TokenClass, path);
            }
            else
            {
                TypeCode expectedType = Type.GetTypeCode(expected.GetType());

                if (expectedType == TypeCode.Object)
                {
                    if (expected.GetType().IsArray)
                        TestArray((Array) expected, reader, path);
                    else
                        TestObject((JsonObject) expected, reader, path);
                }
                else
                {
                    switch (expectedType)
                    {
                        case TypeCode.String : Assert.AreEqual(expected, reader.ReadString(), path); break;
                        case TypeCode.Int32  : Assert.AreEqual(expected, (int) reader.ReadNumber(), path); break;
                        default : Assert.Fail("Don't know how to handle {0} values.", expected.GetType()); break;
                    }
                }
            }
        }

        public sealed class Car
        {
            private string _manufacturer;
            private string _model;
            private int _year;
            private string _color;

            public string Manufacturer
            {
                get { return _manufacturer; }
                set { _manufacturer = value; }
            }

            public string Model
            {
                get { return _model; }
                set { _model = value; }
            }

            public int Year
            {
                get { return _year; }
                set { _year = value; }
            }

            public string Color
            {
                get { return _color; }
                set { _color = value; }
            }
        }

        public sealed class Person
        {
            private int _id;
            private string _fullName;

            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string FullName
            {
                get { return _fullName; }
                set { _fullName = value; }
            }
        }

        public sealed class Marriage
        {
            public Person Husband;
            public Person Wife;
        }
            
        public sealed class OwnerCars
        {
            public Person Owner;
            public ArrayList Cars = new ArrayList();
        }

        public class Point : IJsonExportable
        {
            private int _x;
            private int _y;
            
            private static readonly ICustomTypeDescriptor _componentType;
            
            static Point()
            {
                Type type = typeof(Point);
                PropertyInfo x = type.GetProperty("X");
                PropertyInfo y = type.GetProperty("Y");
                _componentType = new CustomTypeDescriptor(type, new MemberInfo[] { x, y }, new string[] { "x", "y" });
            }

            public Point(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public int X { get { return _x; } set { _x = value; } }
            public int Y { get { return _y; } set { _y = value; } }

            public void Export(ExportContext context, JsonWriter writer)
            {
                ComponentExporter exporter = new ComponentExporter(typeof(Point), _componentType);
                exporter.Export(new ExportContext(), this, writer);
            }
        }
    }
}
