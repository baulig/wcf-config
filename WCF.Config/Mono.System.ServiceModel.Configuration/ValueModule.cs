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

	public abstract class ValueModule<T> : Module<T>
		where T : class, new()
	{
		protected class ValueElement<U,V> : Element<T>
			where U : class, new()
			where V : Module<U>, new()
		{
			public ValueElement (Func<T, U> getter)
				: base (Generator.GetModule<V,U> (), typeof (U))
			{
				this.ValueGetter = getter;
			}
			
			public Func<T, U> ValueGetter {
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
		
		protected virtual ValueElement<U,V> AddElement<U,V> (Func<T, U> getter)
			where U : class, new()
			where V : Module<U>, new()
		{
			var element = new ValueElement<U,V> (getter);
			AddElement (element);
			return element;
		}
		
		protected override void CreateSchemaType (XmlSchemaComplexType type, SchemaTypeMap map)
		{
			if (HasElements) {
				var all = new XmlSchemaAll ();
				all.MinOccurs = 0;
				foreach (var element in Elements) {
					var item = map.CreateModuleElement (element.Module);
					all.Items.Add (item);
				}
				type.Particle = all;
			}
		}
	}
}

