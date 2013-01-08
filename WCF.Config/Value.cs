//
// Value.cs
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

namespace WCF.Config {

	public abstract class Value {
		public virtual bool HasChildren {
			get { return false; }
		}

		public virtual IList<Value> GetChildren ()
		{
			throw new InvalidOperationException ();
		}

		public abstract void Serialize (XmlWriter writer);
	}

	public abstract class Value<TInstance> : Value
		where TInstance : class, new()
	{
		public ValueModule<TInstance> Module {
			get;
			private set;
		}
		
		public TInstance Instance {
			get;
			private set;
		}

		protected Value (ValueModule<TInstance> module, TInstance instance)
		{
			this.Module = module;
			this.Instance = instance;
		}

		protected virtual void DoSerialize (XmlWriter writer)
		{
			if (HasChildren) {
				foreach (var child in GetChildren ())
					child.Serialize (writer);
			}
		}

		public override void Serialize (XmlWriter writer)
		{
			writer.WriteStartElement ("test", Module.Name, Generator.Namespace);
			var defaultInstance = new TInstance ();
			if (Module.HasAttributes) {
				foreach (var attr in Module.Attributes) {
					var value = attr.Func (Instance);
					if (value == null)
						continue;
					if (!attr.IsRequired) {
						var defaultValue = attr.Func (defaultInstance);
						if (object.Equals (value, defaultValue))
							continue;
					}
					writer.WriteAttributeString (attr.Name, value.ToString ());
				}
			}
			DoSerialize (writer);
			writer.WriteEndElement ();
		}
	}

}

