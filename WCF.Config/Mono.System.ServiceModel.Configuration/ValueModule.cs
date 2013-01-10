//
// ValueModule.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

namespace Mono.System.ServiceModel.Configuration {

	public abstract class ValueModule<T> : Module<T>
		where T : class, new()
	{
		protected class ValueElement<U,V> : Element<T>
			where U : class, new()
			where V : Module<U>, new()
		{
			public ValueElement (Func<T, U> getter)
				: base (new V (), typeof (U))
			{
				this.ValueGetter = getter;
			}
			
			public Func<T, U> ValueGetter {
				get;
				private set;
			}

			public Func<T, bool> IsModifiedFunc {
				get;
				private set;
			}

			public ValueElement<U,V> IsModified (Func<T, bool> func)
			{
				IsModifiedFunc = func;
				return this;
			}

			public override void Serialize (XmlWriter writer, T instance)
			{
				var value = ValueGetter (instance);
				if (value == null)
					return;

				if (IsModifiedFunc != null && !IsModifiedFunc (instance))
					return;

				Module.Serialize (writer, value);
			}
			
			public override void Deserialize (XmlReader reader, T instance)
			{
				Module.Deserialize (reader, ValueGetter (instance));
			}
		}
		
		protected virtual ValueElement<U,V> AddElement<U,V> (Func<T, U> getter)
			where U : class, new()
			where V : Module<U>, new()
		{
			var element = new ValueElement<U,V> (getter);
			AddElement (element);
			return element;
		}
		
		internal static XmlTypeCode GetTypeCode (Type type)
		{
			if (type == typeof (string))
				return XmlTypeCode.String;
			else if (type == typeof (bool))
				return XmlTypeCode.Boolean;
			else if (type == typeof (int))
				return XmlTypeCode.Int;
			else if (type == typeof (long))
				return XmlTypeCode.Long;
			else if (type == typeof (TimeSpan))
				return XmlTypeCode.Time;
			else if (type == typeof (Uri))
				return XmlTypeCode.AnyUri;
			else if (type.IsEnum)
				return XmlTypeCode.String;
			else if (type == typeof (Encoding))
				return XmlTypeCode.String;
			else
				throw new ArgumentException (string.Format (
					"Unknown attribute type `{0}'", type));
		}
		
		protected override void CreateSchema (XmlSchemaComplexType type)
		{
			var defInstance = new T ();
			foreach (var attr in Attributes) {
				var xsa = new XmlSchemaAttribute ();
				xsa.Name = attr.Name;
				xsa.Use = attr.IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional;

				type.Attributes.Add (xsa);

				if (!attr.IsRequired)
					xsa.DefaultValue = attr.Serialize (defInstance);

				if (attr.SchemaType != null) {
					xsa.SchemaType = attr.SchemaType;
					continue;
				}

				var tc = GetTypeCode (attr.Type);
				var builtin = XmlSchemaSimpleType.GetBuiltInSimpleType (tc);
				
				var restriction = attr.Content as XmlSchemaSimpleTypeRestriction;
				if (restriction != null) {
					var simple = new XmlSchemaSimpleType ();
					restriction.BaseTypeName = builtin.QualifiedName;
					simple.Content = restriction;
					xsa.SchemaType = simple;
				} else if (attr.Type.IsEnum) {
					xsa.SchemaType = Generator.CreateEnumerationType (attr.Type);
				} else {
					xsa.SchemaTypeName = builtin.QualifiedName;
				}
			}

			if (HasElements) {
				var all = new XmlSchemaAll ();
				all.MinOccurs = 0;
				foreach (var element in Elements) {
					all.Items.Add (element.Module.CreateSchema ());
				}
				type.Particle = all;
			}
				
			base.CreateSchema (type);
		}
	}
}

