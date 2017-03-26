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
    using NUnit.Framework;

    #endregion

    public abstract class TestNumberExporter
    {
        [ Test ]
        public void InputType()
        {
            Assert.AreSame(SampleValue.GetType(), CreateExporter().InputType);
        }

        [ Test ]
        public void ExportNull()
        {
            JsonRecorder writer = new JsonRecorder();
            CreateExporter().Export(new ExportContext(), null, writer);
            writer.CreatePlayer().ReadNull();
        }

        [ Test ]
        public void ExportNumber()
        {
            JsonRecorder writer = new JsonRecorder();
            object sample = SampleValue;
            CreateExporter().Export(new ExportContext(), sample, writer);
            object actual = Convert.ChangeType(writer.CreatePlayer().ReadNumber(), sample.GetType());
            Assert.IsInstanceOfType(sample.GetType(), actual);
            Assert.AreEqual(sample, actual);
        }

        protected abstract object SampleValue { get; }
        protected abstract IExporter CreateExporter();
    }

    [ TestFixture ]
    public class TestByteExporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return (byte) 123; }
        }

        protected override IExporter CreateExporter()
        {
            return new ByteExporter();
        }
    }

    [ TestFixture ]
    public class TestInt16Exporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return (short) 1234; }
        }

        protected override IExporter CreateExporter()
        {
            return new Int16Exporter();
        }
    }

    [ TestFixture ]
    public class TestInt32Exporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return 123456; }
        }

        protected override IExporter CreateExporter()
        {
            return new Int32Exporter();
        }
    }

    [ TestFixture ]
    public class TestInt64Exporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return 9876543210L; }
        }

        protected override IExporter CreateExporter()
        {
            return new Int64Exporter();
        }
    }
    
    [ TestFixture ]
    public class TestSingleExporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return 12.345f; }
        }

        protected override IExporter CreateExporter()
        {
            return new SingleExporter();
        }
    }

    [ TestFixture ]
    public class TestDoubleExporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return 12.345m; }
        }

        protected override IExporter CreateExporter()
        {
            return new DecimalExporter();
        }
    }
}

#if !NET_1_0 && !NET_1_1 && !NET_2_0

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System.Numerics;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestBigIntegerExporter : TestNumberExporter
    {
        protected override object SampleValue
        {
            get { return BigInteger.Pow(long.MaxValue, 3); }
        }

        protected override IExporter CreateExporter()
        {
            return new BigIntegerExporter();
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0
