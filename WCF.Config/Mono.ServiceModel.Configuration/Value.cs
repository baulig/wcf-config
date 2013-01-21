//
// Value.cs
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

namespace Mono.ServiceModel.Configuration {

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

		string Name {
			get;
		}

		bool IsSupported (Context context, T instance);
	}
	
	public abstract class Value<T> : IValue<T>, IAttributeList<T>, IElementList<T>
		where T : class
	{
		List<IElement<T>> elements = new List<IElement<T>> ();
		List<IAttribute<T>> attributes = new List<IAttribute<T>> ();

		public string Name {
			get { return typeof (T).Name; }
		}

		protected void AddElement (IElement<T> element)
		{
			elements.Add (element);
		}

		protected void AddAttribute (IAttribute<T> attribute)
		{
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

		public virtual bool IsSupported (Context context, T instance)
		{
			return true;
		}
		
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
			
			public override void Serialize (Context context, XmlWriter writer, T instance)
			{
				var value = ValueGetter (instance);
				if (value == null)
					return;
				
				Module.Serialize (context, writer, value);
			}
			
			public override void Deserialize (Context context, XmlReader reader, T instance)
			{
				Module.Deserialize (context, reader, ValueGetter (instance));
			}
		}
		
		public Element<T> AddElement<U,V> (Func<T, V> getter)
			where U : Module<V>, new()
			where V : class, new()
		{
			var element = new ValueElement<U,V> (getter);
			elements.Add (element);
			return element;
		}
		
		#region IValue<T> implementation
		
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
		
		#endregion
	}
}

