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

namespace Mono.System.ServiceModel.Configuration {

	public abstract class Module {

		public abstract string Name {
			get;
		}

		internal abstract void RegisterSchemaTypes (SchemaTypeMap map);

		public abstract void Serialize (XmlWriter writer, object obj);

		public abstract void Deserialize (XmlReader reader, object obj);
	}

	public abstract class Module<T> : Module
		where T : class, new()
	{
		List<Element<T>> elements;
		List<Attribute<T>> attributes;
		bool populated;
		
		public bool HasElements {
			get { return elements.Count > 0; }
		}
		
		public IList<Element<T>> Elements {
			get { return elements.AsReadOnly (); }
		}

		protected Module ()
		{
			elements = new List<Element<T>> ();
			attributes = new List<Attribute<T>> ();
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

		protected void AddAttribute (Attribute<T> attribute)
		{
			if (populated)
				throw new InvalidOperationException ();
			attributes.Add (attribute);
		}

		public Attribute<T,U> AddAttribute<U> (string name, Func<T, U> getter, Action<T, U> setter)
		{
			return AddAttribute (name, false, getter, setter);
		}
		
		public Attribute<T,U> AddAttribute<U> (string name, bool required,
		                                       Func<T, U> getter, Action<T, U> setter)
		{
			var attribute = new Attribute<T,U> (name, required, getter, setter);
			AddAttribute (attribute);
			return attribute;
		}

		public bool HasAttributes {
			get { return attributes.Count > 0; }
		}
		
		public IList<Attribute<T>> Attributes {
			get { return attributes.AsReadOnly (); }
		}

		protected abstract void CreateSchemaType (XmlSchemaComplexType type, SchemaTypeMap map);

		internal override void RegisterSchemaTypes (SchemaTypeMap map)
		{
			if (map.IsRegistered (this))
				return;

			var type = new XmlSchemaComplexType ();
			type.Name = Name;
			map.RegisterModule (this, type);

			foreach (var attr in Attributes) {
				attr.RegisterSchemaTypes (map);
			}
			
			foreach (var element in Elements) {
				element.Module.RegisterSchemaTypes (map);
			}

			CreateSchemaType (type, map);
		}
		
		public override void Serialize (XmlWriter writer, object obj)
		{
			writer.WriteStartElement (Name, Generator.Namespace);

			var instance = (T) obj;
			var defaultInstance = new T ();
				
			foreach (var attr in Attributes) {
				var value = attr.Serialize (instance);
				if (value == null)
					continue;
				if (!attr.IsRequired) {
					var defaultValue = attr.Serialize (defaultInstance);
					if (object.Equals (value, defaultValue))
						continue;
				}
				writer.WriteAttributeString (attr.Name, value);
			}
				
			foreach (var element in Elements) {
				element.Serialize (writer, instance);
			}

			writer.WriteEndElement ();
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
				attr.Deserialize (instance, reader.Value);
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

