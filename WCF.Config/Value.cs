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
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

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

		internal static XmlTypeCode GetTypeCode (Type type)
		{
			if (type == typeof (string))
				return XmlTypeCode.String;
			else if (type == typeof (bool))
				return XmlTypeCode.Boolean;
			else if (type == typeof (int))
				return XmlTypeCode.Int;
			else if (type == typeof (long))
				return XmlTypeCode.Long;
			else if (type == typeof (TimeSpan))
				return XmlTypeCode.Time;
			else if (type == typeof (Uri))
				return XmlTypeCode.AnyUri;
			else if (type.IsEnum)
				return XmlTypeCode.String;
			else if (type == typeof (Encoding))
				return XmlTypeCode.String;
			else
				throw new ArgumentException (string.Format (
					"Unknown attribute type `{0}'", type));
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
					var value = attr.Getter (Instance);
					if (value == null)
						continue;
					if (!attr.IsRequired) {
						var defaultValue = attr.Getter (defaultInstance);
						if (object.Equals (value, defaultValue))
							continue;
					}
					writer.WriteAttributeString (attr.Name, SerializeValue (value));
				}
			}
			DoSerialize (writer);
			writer.WriteEndElement ();
		}
	}

}

