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

namespace Mono.System.ServiceModel.Configuration {

	public interface IAttributeList<in T> {
		int Count {
			get;
		}

		IAttribute<T> this [int index] {
			get;
		}
	}

	public interface IElementList<in T> {
		int Count {
			get;
		}

		IElement<T> this [int index] {
			get;
		}
	}

	public interface IValue<in T>
	{
		IAttributeList<T> Attributes {
			get;
		}

		IElementList<T> Elements {
			get;
		}
	}

	public abstract class Value<T> : IValue<T>, IAttributeList<T>, IElementList<T>
		where T : class
	{
		List<Element<T>> elements;
		List<Attribute<T>> attributes;
		readonly bool populated;

		protected Value ()
		{
			elements = new List<Element<T>> ();
			attributes = new List<Attribute<T>> ();
			Populate ();
			populated = true;
		}
		
		public bool HasElements {
			get { return elements.Count > 0; }
		}
		
		public IList<Element<T>> Elements {
			get { return elements.AsReadOnly (); }
		}
		
		protected abstract void Populate ();

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

		IAttributeList<T> IValue<T>.Attributes {
			get { return this; }
		}

		int IAttributeList<T>.Count {
			get { return attributes.Count; }
		}

		IAttribute<T> IAttributeList<T>.this [int index] {
			get { return attributes [index]; }
		}

		IElementList<T> IValue<T>.Elements {
			get { return this; }
		}

		int IElementList<T>.Count {
			get { return elements.Count; }
		}

		IElement<T> IElementList<T>.this [int index] {
			get { return elements [index]; }
		}
	}

	public abstract class ValueModule<T> : Module<T>
		where T : class, new()
	{
		List<IValue<T>> values = new List<IValue<T>> ();

		protected class ValueElement<U,V> : Element<T>
			where U : Module<V>, new()
			where V : class, new()
		{
			public ValueElement (Func<T, V> getter)
				: base (Generator.GetModule<U> (), typeof (V))
			{
				this.ValueGetter = getter;
			}
			
			public Func<T, V> ValueGetter {
				get;
				private set;
			}

			public override void Serialize (XmlWriter writer, T instance)
			{
				var value = ValueGetter (instance);
				if (value == null)
					return;

				Module.Serialize (writer, value);
			}
			
			public override void Deserialize (XmlReader reader, T instance)
			{
				Module.Deserialize (reader, ValueGetter (instance));
			}
		}
		
		protected ValueElement<U,V> AddElement<U,V> (Func<T, V> getter)
			where U : Module<V>, new()
			where V : class, new()
		{
			var element = new ValueElement<U,V> (getter);
			AddElement (element);
			return element;
		}

		protected override void Populate ()
		{
			foreach (var value in values) {
				Console.WriteLine ("POPULATE: {0} {1} {2}", this, value, value.Attributes.Count);
				for (int i = 0; i < value.Attributes.Count; i++) {
					var attr = value.Attributes [i];
					Console.WriteLine ("ATTR: {0}", attr);
					base.AddAttribute (attr);
				}

			}
			base.Populate ();
		}

		protected void Implement<U> ()
			where U : IValue<T>, new()
		{
			values.Add (new U ());
		}
		
		protected override void CreateSchemaType (XmlSchemaComplexType type, SchemaTypeMap map)
		{
			if (HasElements) {
				var all = new XmlSchemaAll ();
				all.MinOccurs = 0;
				foreach (var element in Elements) {
					var item = element.Module.CreateSchemaElement (map);
					all.Items.Add (item);
				}
				type.Particle = all;
			}
		}
	}
}

