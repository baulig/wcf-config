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

namespace Mono.ServiceModel.Configuration {

	/*
	 * IMPORTANT:
	 * 
	 * You must not create any generic subclasses of this or it will
	 * not run on the device!
	 * 
	 */
	abstract class ValueModule<T> : Module<T>
		where T : class, new()
	{
		readonly List<IValue<T>> values = new List<IValue<T>> ();
		readonly DefaultValue defaultValue = new DefaultValue ();

		class DefaultValue : Value<T> {
		}
		
		protected Element<T> AddElement<U,V> (Func<T, V> getter)
			where U : Module<V>, new()
			where V : class, new()
		{
			return defaultValue.AddElement<U,V> (getter);
		}
		
		protected override void Populate ()
		{
			values.Add (defaultValue);
			foreach (var value in values) {
				for (int i = 0; i < value.Attributes.Count; i++)
					AddAttribute (value.Attributes [i]);
				for (int i = 0; i < value.Elements.Count; i++)
					AddElement (value.Elements [i]);
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

		protected override bool IsSupported (Context context, T instance)
		{
			bool ok = true;
			foreach (var value in values) {
				if (value.IsSupported (context, instance))
					continue;
				context.AddError ("Value '{0}' is not supported in current context.", value.Name);
				ok = false;
			}

			return ok;
		}
	}

}

