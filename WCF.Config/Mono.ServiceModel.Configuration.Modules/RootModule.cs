//
// RootModule.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using QName = System.Xml.XmlQualifiedName;

namespace Mono.ServiceModel.Configuration.Modules {

	class RootModule : ValueModule<Configuration> {
		public override string Name {
			get { return "wcf-config"; }
		}

		protected override void Populate ()
		{
			AddElement<BindingsModule,Collection<Binding>> (i => i.Bindings);
			AddElement<EndpointsModule,Collection<Endpoint>> (i => i.Endpoints);
			base.Populate ();
		}

		protected override void CreateSchemaElement (XmlSchemaElement schema, SchemaTypeMap map)
		{
			var bindingsKey = new XmlSchemaKey ();
			bindingsKey.Name = "bindingsKey";
				
			var bindingsKeySelector = new XmlSchemaXPath ();
			bindingsKeySelector.XPath = SchemaTypeMap.Prefix + ":bindings/*";
			bindingsKey.Selector = bindingsKeySelector;
				
			var bindingsKeyField = new XmlSchemaXPath ();
			bindingsKeyField.XPath = "@name";
			bindingsKey.Fields.Add (bindingsKeyField);
				
			schema.Constraints.Add (bindingsKey);

			var endpointsKey = new XmlSchemaKey ();
			endpointsKey.Name = "endpointsKey";

			var endpointsKeySelector = new XmlSchemaXPath ();
			endpointsKeySelector.XPath = SchemaTypeMap.Prefix + ":endpoints/*";
			endpointsKey.Selector = endpointsKeySelector;

			var endpointsKeyField = new XmlSchemaXPath ();
			endpointsKeyField.XPath = "@name";
			endpointsKey.Fields.Add (endpointsKeyField);

			schema.Constraints.Add (endpointsKey);

			var bindingsKeyRef = new XmlSchemaKeyref ();
			bindingsKeyRef.Name = "bindingsKeyRef";
			bindingsKeyRef.Refer = new QName ("bindingsKey", Generator.Namespace);

			var bindingsKeyRefSelector = new XmlSchemaXPath ();
			bindingsKeyRefSelector.XPath = SchemaTypeMap.Prefix + ":endpoints/*";
			bindingsKeyRef.Selector = bindingsKeyRefSelector;

			var bindingsKeyRefField = new XmlSchemaXPath ();
			bindingsKeyRefField.XPath = "@binding";
			bindingsKeyRef.Fields.Add (bindingsKeyRefField);

			schema.Constraints.Add (bindingsKeyRef);

			base.CreateSchemaElement (schema, map);
		}
		
	}
}

