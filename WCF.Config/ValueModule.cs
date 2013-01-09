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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

namespace WCF.Config {

	public abstract class ValueModule<T> : Module<T>
		where T : class, new()
	{
		static IList<Element<T>> elements;

		public override IList<Element<T>> Elements {
			get {
				if (elements != null)
					return elements;
				
				var list = new ElementList<T> ();
				GetElements (list);
				elements = list.AsReadOnly ();
				return elements;
			}
		}
		
		protected virtual void GetElements (ElementList<T> list)
		{
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
		
		internal static string SerializeValue (object value)
		{
			if (value == null)
				return null;
			if (value is bool)
				return (bool)value ? "true" : "false";
			else if (value is Encoding)
				return ((Encoding)value).WebName;
			return value.ToString ();
		}

		protected override void Serialize (XmlWriter writer, T instance)
		{
			var defaultInstance = new T ();

			foreach (var attr in Attributes) {
				var value = attr.Getter (instance);
				if (value == null)
					continue;
				if (!attr.IsRequired) {
					var defaultValue = attr.Getter (defaultInstance);
					if (object.Equals (value, defaultValue))
						continue;
				}
				writer.WriteAttributeString (attr.Name, SerializeValue (value));
			}

			foreach (var element in Elements) {
				var value = element.Getter (instance);
				if (value == null)
					continue;
				element.Module.Serialize (writer, value);
			}
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

				var value = attr.Getter (defInstance);
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

			base.CreateSchema (type);
		}
	}
}

