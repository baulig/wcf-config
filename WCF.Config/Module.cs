//
// Module.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;

namespace WCF.Config {

	public abstract class Module {

		public abstract string Name {
			get;
		}

		protected virtual void CreateSchema (XmlSchemaComplexType type)
		{
		}

		public XmlSchemaElement CreateSchema ()
		{
			var element = new XmlSchemaElement ();
			element.Name = Name;

			var type = new XmlSchemaComplexType ();
			element.SchemaType = type;

			CreateSchema (type);

			return element;
		}

		public abstract void Serialize (XmlWriter writer, object obj);
	}

	public abstract class Module<T> : Module {
		static IList<Attribute<T>> attrs;

		public bool HasElements {
			get { return Elements.Count > 0; }
		}
		
		public abstract IList<Element<T>> Elements {
			get;
		}
		
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

		protected override void CreateSchema (XmlSchemaComplexType type)
		{
			if (HasElements) {
				var all = new XmlSchemaAll ();
				foreach (var element in Elements) {
					all.Items.Add (element.Module.CreateSchema ());
				}
				type.Particle = all;
			}
			
			base.CreateSchema (type);
		}

		public override void Serialize (XmlWriter writer, object obj)
		{
			writer.WriteStartElement ("test", Name, Generator.Namespace);

			Serialize (writer, (T) obj);

			writer.WriteEndElement ();
		}

		protected abstract void Serialize (XmlWriter writer, T instance);

	}
}

