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
using System.Xml.Schema;

namespace WCF.Config {

	public abstract class Attribute<T> {

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

		public XmlSchemaSimpleTypeContent Content {
			get;
			set;
		}

		public XmlSchemaSimpleType SchemaType {
			get;
			set;
		}

		public Attribute<T> SetMinMax (string min, string max)
		{
			var restriction = new XmlSchemaSimpleTypeRestriction ();
			var minFacet = new XmlSchemaMinInclusiveFacet ();
			minFacet.Value = min;
			var maxFacet = new XmlSchemaMaxInclusiveFacet ();
			maxFacet.Value = max;
			restriction.Facets.Add (minFacet);
			restriction.Facets.Add (maxFacet);
			Content = restriction;
			return this;
		}

		public Attribute<T> SetSchemaType (XmlSchemaSimpleType type)
		{
			SchemaType = type;
			return this;
		}

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

	public class Attribute<T,U> : Attribute<T> {

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
			SchemaType = CustomSerializer.SchemaType;
			return this;
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
