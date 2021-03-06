//
// ListModule.cs
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
using System.Xml;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Mono.ServiceModel.Configuration {

	/*
	 * IMPORTANT:
	 * 
	 * You must not create any generic subclasses of this or it will
	 * not run on the device!
	 * 
	 */
	abstract class CollectionModule<T> : Module<Collection<T>>
	{
		protected class CollectionElement<U,V> : Element<Collection<T>>
			where U : Module<V>, new()
			where V : class, T, new()
		{
			public CollectionElement ()
				: base (Generator.GetModule<U> (), typeof (V))
			{
			}
			
			public override void Serialize (Context context, XmlWriter writer, Collection<T> instance)
			{
				foreach (var item in instance) {
					var value = item as V;
					if (value == null)
						continue;
					Module.Serialize (context, writer, value);
				}
			}
			
			public override void Deserialize (Context context, XmlReader reader, Collection<T> instance)
			{
				var item = new V ();
				instance.Add (item);
				Module.Deserialize (context, reader, item);
			}
		}

		protected CollectionElement<U,V> AddElement<U,V> ()
			where U : Module<V>, new()
			where V : class, T, new()
		{
			var element = new CollectionElement<U,V> ();
			AddElement (element);
			return element;
		}

		protected override void CreateSchemaType (XmlSchemaComplexType type, SchemaTypeMap map)
		{
			var sequence = new XmlSchemaSequence ();
			foreach (var element in Elements) {
				var item = element.Module.CreateSchemaElement (map);
				item.MinOccurs = 0;
				item.MaxOccursString = "unbounded";
				sequence.Items.Add (item);
			}
			type.Particle = sequence;
		}

		protected override bool IsSupported (Context context, Collection<T> instance)
		{
			return true;
		}
	}
}

