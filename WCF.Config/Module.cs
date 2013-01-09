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
		List<Element<T>> elements;
		bool populated;
		
		public bool HasElements {
			get { return Elements.Count > 0; }
		}
		
		public IList<Element<T>> Elements {
			get { return elements.AsReadOnly (); }
		}

		protected Module ()
		{
			elements = new List<Element<T>> ();
			Populate ();
			populated = true;
		}
		
		protected virtual void Populate ()
		{
		}

		protected void AddElement (Element<T> element)
		{
			if (populated)
				throw new InvalidOperationException ();
			elements.Add (element);
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
		
		public override void Serialize (XmlWriter writer, object obj)
		{
			writer.WriteStartElement ("test", Name, Generator.Namespace);

			var instance = (T) obj;
			var defaultInstance = new T ();
				
			foreach (var attr in Attributes) {
				var value = attr.GetValue (instance);
				if (value == null)
					continue;
				if (!attr.IsRequired) {
					var defaultValue = attr.GetValue (defaultInstance);
					if (object.Equals (value, defaultValue))
						continue;
				}
				writer.WriteAttributeString (attr.Name, SerializeValue (value));
			}
				
			foreach (var element in Elements) {
				element.Serialize (writer, instance);
			}

			writer.WriteEndElement ();
		}

		protected static object Deserialize (Type type, string value)
		{
			if (type == typeof(bool))
				return bool.Parse (value);
			else if (type == typeof(int))
				return int.Parse (value);
			else if (type == typeof(long))
				return long.Parse (value);
			else if (type == typeof(TimeSpan))
				return TimeSpan.Parse (value);
			else if (type == typeof(string))
				return value;
			else if (type.IsEnum)
				return Enum.Parse (type, value);
			else if (type == typeof(Encoding))
				return Encoding.GetEncoding (value);
			else
				throw new InvalidOperationException ();
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
				object value = Deserialize (attr.Type, reader.Value);
				attr.SetValue (instance, value);
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
				element.Deserialize (reader, instance);
			} while (reader.MoveToContent () != XmlNodeType.EndElement);

			reader.ReadEndElement ();
		}
	}
}

