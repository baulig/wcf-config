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

namespace Mono.System.ServiceModel.Configuration {

	public class SchemaTypeMap {

		Dictionary<Type, XmlSchemaType> map = new Dictionary<Type, XmlSchemaType> ();

		public XmlQualifiedName LookupModule (Module module)
		{
			return map [module.GetType ()].QualifiedName;
		}

		public bool IsRegistered (Module module)
		{
			return map.ContainsKey (module.GetType ());
		}

		public XmlQualifiedName RegisterModule (Module module, XmlSchemaType schema)
		{
			map.Add (module.GetType (), schema);
			return schema.QualifiedName;
		}

		public ICollection<XmlSchemaType> Schemas {
			get { return map.Values; }
		}

		public bool IsRegistered (Type type)
		{
			return map.ContainsKey (type);
		}

		public XmlQualifiedName RegisterType (Type type, XmlSchemaType schema)
		{
			map.Add (type, schema);
			return schema.QualifiedName;
		}

		public XmlQualifiedName LookupType (Type type)
		{
			return map [type].QualifiedName;
		}

	}
}

