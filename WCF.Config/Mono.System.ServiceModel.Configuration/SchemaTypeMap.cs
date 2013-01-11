//
// SchemaTypeMap.cs
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
using System.Xml;
using System.Xml.Schema;
using QName = System.Xml.XmlQualifiedName;

namespace Mono.System.ServiceModel.Configuration {

	public class SchemaTypeMap {

		XmlSchema schema;
		Dictionary<Type, XmlSchemaComplexType> moduleMap = new Dictionary<Type, XmlSchemaComplexType> ();
		Dictionary<Type, XmlSchemaSimpleType> typeMap = new Dictionary<Type, XmlSchemaSimpleType> ();

		SchemaTypeMap (Module root)
		{
			schema = new XmlSchema ();
			schema.ElementFormDefault = XmlSchemaForm.Qualified;
			schema.AttributeFormDefault = XmlSchemaForm.Unqualified;
			schema.TargetNamespace = Generator.Namespace;

			schema.Namespaces.Add (Prefix, Generator.Namespace);

			root.RegisterSchemaTypes (this);

			root.CreateSchemaType (this);

			var rootElement = root.CreateSchemaElement (this);
			schema.Items.Add (rootElement);
		}

		public static XmlSchema CreateSchema (Module root)
		{
			return new SchemaTypeMap (root).Schema;
		}

		public XmlSchema Schema {
			get { return schema; }
		}

		public const string Prefix = "mwc";

		public bool IsRegistered (Module module)
		{
			return moduleMap.ContainsKey (module.GetType ());
		}

		public void RegisterModule (Module module, XmlSchemaComplexType type)
		{
			moduleMap.Add (module.GetType (), type);
			schema.Items.Add (type);
		}

		public QName LookupModuleTypeName (Module module)
		{
			var type = moduleMap [module.GetType ()];
			return new QName (type.Name, schema.TargetNamespace);
		}
		
		public bool IsRegistered (Type type)
		{
			return typeMap.ContainsKey (type);
		}

		public QName RegisterType (Type type, XmlSchemaSimpleType item)
		{
			typeMap.Add (type, item);
			schema.Items.Add (item);
			return new QName (item.Name, schema.TargetNamespace);
		}

		public QName LookupTypeName (Type type)
		{
			return new QName (typeMap [type].Name, schema.TargetNamespace);
		}

	}
}

