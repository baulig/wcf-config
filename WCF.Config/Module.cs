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
using System.Text;
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

		public abstract void Deserialize (XmlReader reader, object obj);
	}

	public abstract class Module<T> : Module
		where T : class, new()
	{
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

		public override void Serialize (XmlWriter writer, object obj)
		{
			writer.WriteStartElement ("test", Name, Generator.Namespace);

			Serialize (writer, (T) obj);

			writer.WriteEndElement ();
		}

		protected static bool Deserialize (Type type, string value, out object result)
		{
			if (type == typeof(bool)) {
				result = bool.Parse (value);
				return true;
			} else if (type == typeof(int)) {
				result = int.Parse (value);
				return true;
			} else if (type == typeof(long)) {
				result = long.Parse (value);
				return true;
			} else if (type == typeof(TimeSpan)) {
				result = TimeSpan.Parse (value);
				return true;
			} else if (type == typeof(string)) {
				result = value;
				return true;
			} else if (type.IsEnum) {
				result = Enum.Parse (type, value);
				return true;
			} else if (type == typeof(Encoding)) {
				result = Encoding.GetEncoding (value);
				return true;
			}

			Console.WriteLine ("UNKNOWN TYPE: {0}", type);
			result = null;
			return false;
		}

		public override void Deserialize (XmlReader reader, object obj)
		{
			var instance = (T)obj;
			Deserialize (reader, instance);
		}

		void Deserialize (XmlReader reader, T instance)
		{
			while (reader.MoveToNextAttribute ()) {
				var attr = Attributes.First (t => t.Name.Equals (reader.LocalName));
				object value;
				if (Deserialize (attr.Type, reader.Value, out value))
					attr.Setter (instance, value);
			}

			reader.ReadStartElement (Name, Generator.Namespace);

			if (reader.MoveToContent () == XmlNodeType.EndElement)
				return;

			do {
				if (reader.NodeType == XmlNodeType.EndElement)
					break;
				if (reader.NodeType != XmlNodeType.Element) {
					reader.Skip ();
					continue;
				}

				var element = Elements.First (t => t.Module.Name.Equals (reader.LocalName));
				Deserialize (reader, instance, element);
			} while (reader.MoveToContent () != XmlNodeType.EndElement);

			reader.ReadEndElement ();
		}

		protected abstract void Deserialize (XmlReader reader, T instance, Element<T> element);

		protected abstract void Serialize (XmlWriter writer, T instance);

	}
}

