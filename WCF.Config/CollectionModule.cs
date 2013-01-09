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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCF.Config {

	public abstract class CollectionModule<T> : Module<List<T>>
	{
		protected class CollectionElement<U,V> : Element<List<T>>
			where U : class, T, new()
			where V : Module<U>, new()
		{
			public CollectionElement ()
				: base (new V (), typeof (U))
			{
			}
			
			public override void Serialize (XmlWriter writer, List<T> instance)
			{
				foreach (var item in instance) {
					var value = item as U;
					if (value == null)
						continue;
					Module.Serialize (writer, value);
				}
			}
			
			public override void Deserialize (XmlReader reader, List<T> instance)
			{
				var item = new U ();
				instance.Add (item);
				Module.Deserialize (reader, item);
			}
		}

		protected CollectionElement<U,V> AddElement<U,V> ()
			where U : class, T, new()
			where V : Module<U>, new()
		{
			var element = new CollectionElement<U,V> ();
			AddElement (element);
			return element;
		}

		protected override void CreateSchema (XmlSchemaComplexType type)
		{
			var sequence = new XmlSchemaSequence ();
			foreach (var element in Elements) {
				var item = element.Module.CreateSchema ();
				item.MinOccurs = 0;
				item.MaxOccursString = "unbounded";
				sequence.Items.Add (item);
			}
			type.Particle = sequence;

			base.CreateSchema (type);
		}
	}
}

