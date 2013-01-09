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

namespace WCF.Config {

	public abstract class ValueModule<T> : Module<T>
		where T : class, new()
	{
		protected virtual ValueElement<T,U,V> AddElement<U,V> (Func<T, U> getter)
			where U : class, new()
				where V : Module<U>, new()
		{
			var element = new ValueElement<T,U,V> (getter);
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
		
		XmlSchemaSimpleType CreateEnumerationType (XmlSchemaSimpleType baseType, Type type)
		{
			var simple = new XmlSchemaSimpleType ();

			var restriction = new XmlSchemaSimpleTypeRestriction ();
			restriction.BaseTypeName = baseType.QualifiedName;

			simple.Content = restriction;

			foreach (var member in Enum.GetNames (type)) {
				var facet = new XmlSchemaEnumerationFacet ();
				facet.Value = member;
				restriction.Facets.Add (facet);
			}

			return simple;
		}

		protected override void CreateSchema (XmlSchemaComplexType type)
		{
			var defInstance = new T ();
			foreach (var attr in Attributes) {
				var xsa = new XmlSchemaAttribute ();
				xsa.Name = attr.Name;
				xsa.Use = attr.IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional;

				type.Attributes.Add (xsa);

				var value = attr.GetValue (defInstance);
				if (!attr.IsRequired)
					xsa.DefaultValue = SerializeValue (value);

				var tc = GetTypeCode (attr.Type);
				var builtin = XmlSchemaSimpleType.GetBuiltInSimpleType (tc);
				
				var restriction = attr.Content as XmlSchemaSimpleTypeRestriction;
				if (restriction != null) {
					var simple = new XmlSchemaSimpleType ();
					restriction.BaseTypeName = builtin.QualifiedName;
					simple.Content = restriction;
					xsa.SchemaType = simple;
				} else if (attr.Type.IsEnum) {
					xsa.SchemaType = CreateEnumerationType (builtin, attr.Type);
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

