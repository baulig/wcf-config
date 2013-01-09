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
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

namespace WCF.Config {

	public abstract class ValueModule<T> : Module
		where T : class, new()
	{
		IList<Attribute<T>> attrs;
		IList<Element<T>> elements;

		public bool HasAttributes {
			get { return Attributes.Count > 0; }
		}
			
		public IList<Attribute<T>> Attributes {
			get {
				if (attrs != null)
					return attrs;

				var list = new AttributeList<T> ();
				GetAttributes (list);
				attrs = list.AsReadOnly ();
				return attrs;
			}
		}

		protected virtual void GetAttributes (AttributeList<T> list)
		{
		}

		public bool HasElements {
			get { return Elements.Count > 0; }
		}

		public IList<Element<T>> Elements {
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

		public override bool IsSupported (object instance)
		{
			return instance is T;
		}

		public abstract Value<T> GetValue (T instance);

		public override void Serialize (XmlWriter writer, object obj)
		{
			var instance = (T)obj;
			var defaultInstance = new T ();

			writer.WriteStartElement ("test", Name, Generator.Namespace);

			foreach (var attr in Attributes) {
				var value = attr.Getter (instance);
				if (value == null)
					continue;
				if (!attr.IsRequired) {
					var defaultValue = attr.Getter (defaultInstance);
					if (object.Equals (value, defaultValue))
						continue;
				}
				writer.WriteAttributeString (attr.Name, Value.SerializeValue (value));
			}

			foreach (var element in Elements) {
				var value = element.Getter (instance);
				if (value == null)
					continue;
				element.Module.Serialize (writer, value);
			}

			writer.WriteEndElement ();
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
					xsa.DefaultValue = Value.SerializeValue (value);

				var tc = Value.GetTypeCode (attr.Type);
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

			if (!HasElements)
				return;

			var all = new XmlSchemaAll ();
			all.MinOccurs = 0;

			foreach (var element in Elements) {
				all.Items.Add (element.Module.CreateSchema ());
			}

			type.Particle = all;
		}
	}
}

