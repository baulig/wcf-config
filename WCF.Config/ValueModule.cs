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
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

namespace WCF.Config {

	public abstract class ValueModule<T> : Module
		where T : class
	{
		public abstract bool HasAttributes {
			get;
		}
			
		public abstract IList<Attribute<T>> Attributes {
			get;
		}

		public override bool IsSupported (object instance)
		{
			return instance is T;
		}

		public abstract Value<T> GetValue (T instance);

		public override void Serialize (XmlWriter writer, object instance)
		{
			var value = GetValue ((T)instance);
			value.Serialize (writer);
		}
			
		protected override void CreateSchema (XmlSchemaComplexType type)
		{
			if (!HasAttributes)
				return;
				
			foreach (var attr in Attributes) {
				var xsa = new XmlSchemaAttribute ();
				xsa.Name = attr.Name;
				xsa.Use = attr.IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional;
					
				type.Attributes.Add (xsa);
			}
		}
	}
}

