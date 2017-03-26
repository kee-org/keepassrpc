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

namespace Jayrock.Dynamic
{
    #if !NET_1_0 && !NET_1_1 && !NET_2_0

    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    #endregion

    internal static class Option
    {
        public static Option<T> Value<T>(T value)
        {
            return new Option<T>(true, value);
        }
    }

    /// <summary>
    /// Represents an optional value, somewhat like
    /// <a href="http://msdn.microsoft.com/en-us/library/dd233245.aspx">the option type in F#</a>.
    /// </summary>
    [Serializable]
    internal struct Option<T> : IEquatable<Option<T>>
    {
        // warning CS0649: Field 'Jayrock.Dynamic.Option<T>.None' is never assigned to, and will always have its default value 
        // ReSharper disable RedundantDefaultFieldInitializer
        public static readonly Option<T> None = new Option<T>();
        // ReSharper restore RedundantDefaultFieldInitializer

        private readonly T _value;

        internal Option(bool hasValue, T value)
            : this()
        {
            HasValue = hasValue;
            _value = value;
        }

        public bool HasValue { get; private set; }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Value is undefined.");
                return _value;
            }
        }

        public bool Equals(Option<T> other)
        {
            return HasValue == other.HasValue
                && EqualityComparer<T>.Default.Equals(_value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T> && Equals((Option<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked { return HasValue.GetHashCode() ^ (_value.GetHashCode() * 397); }
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return HasValue
                 ? string.Format("{0}", Value)
                 : string.Empty;
        }
    }

    internal sealed class DynamicObjectRuntime<T>
    {
        public Func<T, GetMemberBinder, Option<object>> TryGetMember { get; set; }
        public Func<T, SetMemberBinder, object, bool> TrySetMember { get; set; }
        public Func<T, DeleteMemberBinder, bool> TryDeleteMember { get; set; }
        public Func<T, InvokeMemberBinder, object[], Option<object>> TryInvokeMember { get; set; }
        public Func<T, ConvertBinder, object[], Option<object>> TryConvert { get; set; }
        public Func<T, CreateInstanceBinder, object[], Option<object>> TryCreateInstance { get; set; }
        public Func<T, InvokeBinder, object[], Option<object>> TryInvoke { get; set; }
        public Func<T, BinaryOperationBinder, object, Option<object>> TryBinaryOperation { get; set; }
        public Func<T, UnaryOperationBinder, Option<object>> TryUnaryOperation { get; set; }
        public Func<T, GetIndexBinder, object[], Option<object>> TryGetIndex { get; set; }
        public Func<T, SetIndexBinder, object[], object> TrySetIndex { get; set; }
        public Func<T, DeleteIndexBinder, object[], Option<object>> TryDeleteIndex { get; set; }
        public Func<T, IEnumerable<string>> GetDynamicMemberNames { get; set; }
    }
}

/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

namespace Jayrock.Dynamic
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion

    /// <remarks>
    /// This DMO implementation has been adapted from that of 
    /// System.Dynamic.DynamicObject, from the CodePlex DLR 1.0 sources, 
    /// and therefore licensed under the terms and conditions of
    /// <a href="http://www.opensource.org/licenses/ms-pl.html">Ms-PL</a>.
    /// </remarks>

    internal sealed class DynamicMetaObject<T> : DynamicMetaObject
    {
        private readonly DynamicObjectRuntime<T> _runtime;
        private readonly bool _dontFallbackFirst;

        internal DynamicMetaObject(Expression expression, T value, DynamicObjectRuntime<T> runtime, bool dontFallbackFirst = false)
            : base(expression, BindingRestrictions.Empty, value)
        {
            Debug.Assert(runtime != null);
            _runtime = runtime; // TODO Consider a read-only shadow
            _dontFallbackFirst = dontFallbackFirst;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _runtime.GetDynamicMemberNames(Value);
        }

        private new T Value { get { return (T) base.Value; } }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            return _runtime.TryGetMember != null
                 ? CallMethodWithResult("TryGetMember", binder, NoArgs, e => binder.FallbackGetMember(this, e))
                 : base.BindGetMember(binder);
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            return _runtime.TrySetMember != null
                 ? CallMethodReturnLast("TrySetMember", binder, GetArgs(value), e => binder.FallbackSetMember(this, value, e))
                 : base.BindSetMember(binder, value);
        }

        public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        {
            return _runtime.TryDeleteMember != null
                 ? CallMethodNoResult("TryDeleteMember", binder, NoArgs, e => binder.FallbackDeleteMember(this, e))
                 : base.BindDeleteMember(binder);
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            return _runtime.TryConvert != null 
                 ? CallMethodWithResult("TryConvert", binder, NoArgs, e => binder.FallbackConvert(this, e)) 
                 : base.BindConvert(binder);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (_runtime.TryInvokeMember == null)
                return base.BindInvokeMember(binder, args);

            //
            // Generate a tree like:
            //
            // {
            //   object result;
            //   TryInvokeMember(payload, out result)
            //      ? result
            //      : TryGetMember(payload, out result)
            //          ? FallbackInvoke(result)
            //          : fallbackResult
            // }
            //
            // Then it calls FallbackInvokeMember with this tree as the
            // "error", giving the language the option of using this
            // tree or doing .NET binding.
            //
            Fallback fallback = e => binder.FallbackInvokeMember(this, args, e);

            var call = BuildCallMethodWithResult(
                "TryInvokeMember",
                binder,
                GetArgArray(args),
                BuildCallMethodWithResult(
                    "TryGetMember",
                    new GetBinderAdapter(binder),
                    NoArgs,
                    fallback(null),
                    e => binder.FallbackInvoke(e, args, null)
                ),
                null
            );
            
            return _dontFallbackFirst ? call : fallback(call);
            //
            // See http://gist.github.com/386261 for why this is commented out
            //
            // IronRuby 1.0.0.0 on .NET 4.0.30319.1
            // Copyright (c) Microsoft Corporation. All rights reserved.
            //
            // >>> require 'Jayrock.Json'
            // => true
            // >>> o = Jayrock::Json::JsonObject.new
            // => {}
            // >>> o.foo = 123
            // => 123
            // >>> o.foo
            // (ir):1: undefined method `foo' for {"foo":123}:Jayrock::Json::JsonObject (NoMethodError)
        }


        public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
        {
            return _runtime.TryCreateInstance != null
                 ? CallMethodWithResult("TryCreateInstance", binder, GetArgArray(args), e => binder.FallbackCreateInstance(this, args, e)) 
                 : base.BindCreateInstance(binder, args);
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            return _runtime.TryInvoke != null ? CallMethodWithResult("TryInvoke", binder, GetArgArray(args), e => binder.FallbackInvoke(this, args, e)) : base.BindInvoke(binder, args);
        }

        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        {
            if (_runtime.TryBinaryOperation != null)
            {
                return CallMethodWithResult("TryBinaryOperation", binder, GetArgs(arg), e => binder.FallbackBinaryOperation(this, arg, e));
            }

            return base.BindBinaryOperation(binder, arg);
        }

        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        {
            return _runtime.TryUnaryOperation != null 
                 ? CallMethodWithResult("TryUnaryOperation", binder, NoArgs, e => binder.FallbackUnaryOperation(this, e)) 
                 : base.BindUnaryOperation(binder);
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            return _runtime.TryGetIndex != null 
                 ? CallMethodWithResult("TryGetIndex", binder, GetArgArray(indexes), e => binder.FallbackGetIndex(this, indexes, e)) 
                 : base.BindGetIndex(binder, indexes);
        }

        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            return _runtime.TrySetIndex != null 
                 ? CallMethodReturnLast("TrySetIndex", binder, GetArgArray(indexes, value), e => binder.FallbackSetIndex(this, indexes, value, e)) 
                 : base.BindSetIndex(binder, indexes, value);
        }

        public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
        {
            return _runtime.TryDeleteIndex != null 
                 ? CallMethodNoResult("TryDeleteIndex", binder, GetArgArray(indexes), e => binder.FallbackDeleteIndex(this, indexes, e)) 
                 : base.BindDeleteIndex(binder, indexes);
        }

        private delegate DynamicMetaObject Fallback(DynamicMetaObject errorSuggestion);

        private readonly static Expression[] NoArgs = new Expression[0];

        private static Expression[] GetArgs(params DynamicMetaObject[] args)
        {
            return args.Select(arg => Expression.Convert(arg.Expression, typeof(object))).ToArray();
        }

        private static Expression[] GetArgArray(DynamicMetaObject[] args)
        {
            return new[] { Expression.NewArrayInit(typeof(object), GetArgs(args)) };
        }

        private static Expression[] GetArgArray(DynamicMetaObject[] args, DynamicMetaObject value)
        {
            return new Expression[] 
            {
                Expression.NewArrayInit(typeof(object), GetArgs(args)),
                Expression.Convert(value.Expression, typeof(object))
            };
        }

        private static ConstantExpression Constant(DynamicMetaObjectBinder binder)
        {
            var t = binder.GetType();
            while (!t.IsVisible)
                t = t.BaseType;
            return Expression.Constant(binder, t);
        }

        /// <summary>
        /// Helper method for generating a MetaObject which calls a
        /// specific method on Dynamic that returns a result
        /// </summary>
        private DynamicMetaObject CallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback, Fallback fallbackInvoke = null)
        {
            //
            // First, call fallback to do default binding
            // This produces either an error or a call to a .NET member
            //
            var fallbackResult = fallback(null);

            var callDynamic = BuildCallMethodWithResult(methodName, binder, args, fallbackResult, fallbackInvoke);

            //
            // Now, call fallback again using our new MO as the error
            // When we do this, one of two things can happen:
            //   1. Binding will succeed, and it will ignore our call to
            //      the dynamic method, OR
            //   2. Binding will fail, and it will use the MO we created
            //      above.
            //

            return _dontFallbackFirst ? callDynamic : fallback(callDynamic);
            //
            // See http://gist.github.com/386261 for why this is commented out
            //
            // IronRuby 1.0.0.0 on .NET 4.0.30319.1
            // Copyright (c) Microsoft Corporation. All rights reserved.
            //
            // >>> require 'Jayrock.Json'
            // => true
            // >>> o = Jayrock::Json::JsonObject.new
            // => {}
            // >>> o.foo = 123
            // => 123
            // >>> o.foo
            // (ir):1: undefined method `foo' for {"foo":123}:Jayrock::Json::JsonObject (NoMethodError)
        }

        private DynamicMetaObject BuildCallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, DynamicMetaObject fallbackResult, Fallback fallbackInvoke)
        {
            //
            // Build a new expression like:
            // {
            //   object result;
            //   TryGetMember(payload, out result) ? fallbackInvoke(result) : fallbackResult
            // }
            // {
            //   Option<object> result;
            //   result = TryGetMember(payload, out result) 
            //   result.HasValue ? fallbackInvoke(result.Value) : fallbackResult
            // }
            //
            var result = Expression.Parameter(typeof(Option<object>), null);
            var callArgs = new Expression[] { Expression.Convert(Expression, typeof(T)), Constant(binder) }.Concat(args);

            var resultDmo = new DynamicMetaObject(Expression.MakeMemberAccess(
                                 result,
                                 typeof(Option<object>).GetProperty("Value")
                             ), BindingRestrictions.Empty);

            // Need to add a conversion if calling TryConvert
            if (binder.ReturnType != typeof(object))
            {
                Debug.Assert(binder is ConvertBinder && fallbackInvoke == null);

                var convert = Expression.Convert(resultDmo.Expression, binder.ReturnType);
                // will always be a cast or unbox
                Debug.Assert(convert.Method == null);

                resultDmo = new DynamicMetaObject(convert, resultDmo.Restrictions);
            }

            if (fallbackInvoke != null)
                resultDmo = fallbackInvoke(resultDmo);

            var callDynamic = new DynamicMetaObject(
                Expression.Block(
                    new[] { result },
                    Expression.Assign(
                        result,
                        Expression.Invoke(
                            Expression.MakeMemberAccess(
                                Expression.Constant(_runtime),
                                typeof(DynamicObjectRuntime<T>).GetProperty(methodName)),
                            callArgs
                        )),
                    Expression.Condition(
                        Expression.MakeMemberAccess(
                            result,
                            typeof(Option<object>).GetProperty("HasValue")
                        ),
                        resultDmo.Expression,
                        fallbackResult.Expression,
                        binder.ReturnType
                    )
                ),
                GetRestrictions().Merge(resultDmo.Restrictions).Merge(fallbackResult.Restrictions)
            );
            return callDynamic;
        }

        /// <summary>
        /// Helper method for generating a MetaObject which calls a
        /// specific method on Dynamic, but uses one of the arguments for
        /// the result.
        /// </summary>
        private DynamicMetaObject CallMethodReturnLast(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback)
        {
            //
            // First, call fallback to do default binding
            // This produces either an error or a call to a .NET member
            //
            var fallbackResult = fallback(null);

            //
            // Build a new expression like:
            // {
            //   object result;
            //   TrySetMember(payload, result = value) ? result : fallbackResult
            // }
            //
            var result = Expression.Parameter(typeof(object), null);
            var callArgs = new Expression[] { Expression.Convert(Expression, typeof(T)), Constant(binder), }.Concat(args);

            var callDynamic = new DynamicMetaObject(
                Expression.Block(
                    new[] { result },
                    Expression.Condition(
                        Expression.Invoke(
                            Expression.MakeMemberAccess(
                                Expression.Constant(_runtime),
                                typeof(DynamicObjectRuntime<T>).GetProperty(methodName)),
                            callArgs
                        ),
                        result,
                        fallbackResult.Expression,
                        typeof(object)
                    )
                ),
                GetRestrictions().Merge(fallbackResult.Restrictions)
            );

            //
            // Now, call fallback again using our new MO as the error
            // When we do this, one of two things can happen:
            //   1. Binding will succeed, and it will ignore our call to
            //      the dynamic method, OR
            //   2. Binding will fail, and it will use the MO we created
            //      above.
            //
            return _dontFallbackFirst ? callDynamic : fallback(callDynamic);
        }


        /// <summary>
        /// Helper method for generating a MetaObject which calls a
        /// specific method on Dynamic, but uses one of the arguments for
        /// the result.
        /// </summary>
        private DynamicMetaObject CallMethodNoResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback)
        {
            //
            // First, call fallback to do default binding
            // This produces either an error or a call to a .NET member
            //
            var fallbackResult = fallback(null);

            //
            // Build a new expression like:
            //   if (TryDeleteMember(payload)) { } else { fallbackResult }
            //
            var callDynamic = new DynamicMetaObject(
                Expression.Condition(
                    Expression.Invoke(
                        Expression.MakeMemberAccess(
                            Expression.Constant(_runtime),
                            typeof(DynamicObjectRuntime<T>).GetProperty(methodName)),
                        new Expression[] { Expression.Convert(Expression, typeof(T)), Constant(binder) }.Concat(args)
                    ),
                    Expression.Empty(),
                    fallbackResult.Expression,
                    typeof(void)
                ),
                GetRestrictions().Merge(fallbackResult.Restrictions)
            );

            //
            // Now, call fallback again using our new MO as the error
            // When we do this, one of two things can happen:
            //   1. Binding will succeed, and it will ignore our call to
            //      the dynamic method, OR
            //   2. Binding will fail, and it will use the MO we created
            //      above.
            //
            return _dontFallbackFirst ? callDynamic : fallback(callDynamic);
        }

        /// <summary>
        /// Returns a Restrictions object which includes our current restrictions merged
        /// with a restriction limiting our type
        /// </summary>
        private BindingRestrictions GetRestrictions()
        {
            Debug.Assert(Restrictions == BindingRestrictions.Empty, "We don't merge, restrictions are always empty");

            // ReSharper disable CompareNonConstrainedGenericWithNull
            return Value == null && HasValue // ReSharper restore CompareNonConstrainedGenericWithNull
                 ? BindingRestrictions.GetInstanceRestriction(Expression, null)
                 : BindingRestrictions.GetTypeRestriction(Expression, LimitType);
        }

        // It is okay to throw NotSupported from this binder. This object
        // is only used by DynamicObject.GetMember--it is not expected to
        // (and cannot) implement binding semantics. It is just so the DO
        // can use the Name and IgnoreCase properties.
        private sealed class GetBinderAdapter : GetMemberBinder
        {
            internal GetBinderAdapter(InvokeMemberBinder binder) : 
                base(binder.Name, binder.IgnoreCase) {}

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotSupportedException();
            }
        }
    }
    
    #endif // !NET_1_0 && !NET_1_1 && !NET_2_0
}
