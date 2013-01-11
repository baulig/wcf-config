//
// KeyedCollectionModule.cs
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

namespace Mono.System.ServiceModel.Configuration {

	public interface IKey {
		string ID {
			get; set;
		}
	}

	public abstract class KeyedCollectionModule<T,U> : Module<Collection<U>>
		where T : KeyedValueModule<U>, new()
		where U : class, IKey, new()
	{
		CollectionElement element;

		class CollectionElement : Element<Collection<U>>
		{
			public CollectionElement ()
				: base (Generator.GetModule<T,U> (), typeof (U))
			{
			}
			
			public override void Serialize (XmlWriter writer, Collection<U> instance)
			{
				foreach (var item in instance) {
					Module.Serialize (writer, item);
				}
			}
			
			public override void Deserialize (XmlReader reader, Collection<U> instance)
			{
				var item = new U ();
				instance.Add (item);
				Module.Deserialize (reader, item);
			}
		}

		protected override void Populate ()
		{
			element = new CollectionElement ();
			AddElement (element);
			base.Populate ();
		}

		internal override void CreateSchemaElement (XmlSchemaElement schema, SchemaTypeMap map)
		{
			var key = new XmlSchemaKey ();
			key.Name = "idKey_" + Name;
			
			var selector = new XmlSchemaXPath ();
			selector.XPath = SchemaTypeMap.Prefix + ":" + element.Module.Name;
			key.Selector = selector;
			
			var field = new XmlSchemaXPath ();
			field.XPath = "@ID";
			key.Fields.Add (field);
			
			schema.Constraints.Add (key);

			base.CreateSchemaElement (schema, map);
		}
		
		protected override void CreateSchemaType (XmlSchemaComplexType type, SchemaTypeMap map)
		{
			var sequence = new XmlSchemaSequence ();

			var item = map.CreateModuleElement (element.Module);
			item.MinOccurs = 0;
			item.MaxOccursString = "unbounded";
			sequence.Items.Add (item);

			type.Particle = sequence;
		}

	}
}

