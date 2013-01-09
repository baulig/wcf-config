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

	public class Attribute<T>
		where T : class, new()
	{
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

		public Func<T, object> Getter {
			get;
			private set;
		}

		public Action<T, object> Setter {
			get;
			private set;
		}

		public XmlSchemaSimpleTypeContent Content {
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

		public Attribute (string name, Type type, Func<T, object> getter, Action<T, object> setter)
			: this (name, type, false, getter, setter)
		{ }

		public Attribute (string name, Type type, bool required,
		                  Func<T, object> getter, Action<T, object> setter)
		{
			this.Name = name;
			this.Type = type;
			this.Getter = getter;
			this.Setter = setter;
			this.IsRequired = required;
		}
	}
}
