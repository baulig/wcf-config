//
// Attribute.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://www.xamarin.com)
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Mono.ServiceModel.Configuration {

	interface IAttribute<in T> {
		string Name {
			get;
		}
		
		Type Type {
			get;
		}
		
		bool IsRequired {
			get;
		}

		XmlSchemaAttribute CreateSchema (T defaultInstance);
		
		void RegisterSchemaTypes (SchemaTypeMap map);

		void Deserialize (T instance, string text);
		
		string Serialize (T instance);
	}

	abstract class Attribute<T> : IAttribute<T> {

		public string Name {
			get;
			private set;
		}

		public Type Type {
			get;
			private set;
		}
		
		public bool IsRequired {
			get;
			private set;
		}

		protected XmlSchemaSimpleTypeRestriction Restriction {
			get;
			private set;
		}

		public Attribute<T> SetMinMax (string min, string max)
		{
			Restriction = new XmlSchemaSimpleTypeRestriction ();
			var minFacet = new XmlSchemaMinInclusiveFacet ();
			minFacet.Value = min;
			var maxFacet = new XmlSchemaMaxInclusiveFacet ();
			maxFacet.Value = max;
			Restriction.Facets.Add (minFacet);
			Restriction.Facets.Add (maxFacet);
			return this;
		}

		public abstract XmlSchemaAttribute CreateSchema (T defaultInstance);

		public abstract void RegisterSchemaTypes (SchemaTypeMap map);

		public abstract void Deserialize (T instance, string text);

		public abstract string Serialize (T instance);

		public Attribute (string name, Type type)
			: this (name, type, false)
		{ }

		public Attribute (string name, Type type, bool required)
		{
			this.Name = name;
			this.Type = type;
			this.IsRequired = required;
		}
	}

	class Attribute<T,U> : Attribute<T>
	{
		public Attribute (string name, Func<T, U> getter, Action<T, U> setter)
			: this (name, false, getter, setter)
		{ }
		
		public Attribute (string name, bool required,
		                  Func<T, U> getter, Action<T, U> setter)
			: base (name, typeof (U), required)
		{
			this.Getter = getter;
			this.Setter = setter;
		}

		public Func<T, U> Getter {
			get;
			private set;
		}
		
		public Action<T, U> Setter {
			get;
			private set;
		}

		public ValueSerializer<U> CustomSerializer {
			get;
			set;
		}
		
		public Attribute<T,U> SetCustomSerializer<V> ()
			where V : ValueSerializer<U>, new()
		{
			CustomSerializer = new V ();
			return this;
		}

		XmlSchemaSimpleType schemaType;
		XmlQualifiedName schemaTypeName;

		public override void RegisterSchemaTypes (SchemaTypeMap map)
		{
			if (Type.IsEnum) {
				schemaTypeName = Generator.GetEnumerationType (Type, map);
				return;
			}

			if (CustomSerializer != null) {
				schemaTypeName = CustomSerializer.GetSchemaType (map);
				return;
			}

			var tc = Generator.GetTypeCode (Type);
			var builtin = XmlSchemaSimpleType.GetBuiltInSimpleType (tc);

			if (Restriction != null) {
				var simple = new XmlSchemaSimpleType ();
				Restriction.BaseTypeName = builtin.QualifiedName;
				simple.Content = Restriction;
				schemaType = simple;
			} else {
				schemaTypeName = builtin.QualifiedName;
			}
		}

		public override XmlSchemaAttribute CreateSchema (T defaultInstance)
		{
			var xsa = new XmlSchemaAttribute ();
			xsa.Name = Name;
			xsa.Use = IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional;
			
			if (!IsRequired)
				xsa.DefaultValue = Serialize (defaultInstance);

			if (schemaType != null)
				xsa.SchemaType = schemaType;
			else if (schemaTypeName != null)
				xsa.SchemaTypeName = schemaTypeName;
			else
				throw new InvalidOperationException ();

			return xsa;
		}

		public override string Serialize (T instance)
		{
			var value = Getter (instance);
			if (CustomSerializer != null)
				return CustomSerializer.Serialize (value);
			else
				return Generator.SerializeValue (value);
		}

		public override void Deserialize (T instance, string text)
		{
			U value;
			if (CustomSerializer != null)
				value = CustomSerializer.Deserialize (text);
			else
				value = Generator.DeserializeValue<U> (text);
			Setter (instance, value);
		}
		

	}
}
