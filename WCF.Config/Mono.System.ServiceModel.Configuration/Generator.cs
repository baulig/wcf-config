//
// Generator.cs
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
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Mono.System.ServiceModel.Configuration {

	using Modules;

	public class Generator {
		static readonly RootModule rootModule = new RootModule ();

		public const string Namespace = "https://github.com/baulig/wcf-config/schema";

		public static XmlSchema CreateSchema ()
		{
			var schema = new XmlSchema ();
			schema.TargetNamespace = Namespace;

			var modules = new List<Module> ();
			rootModule.RegisterChildModules (modules);

			foreach (var module in modules) {
				var type = module.CreateSchemaType ();
				schema.Items.Add (type);
				Console.WriteLine (module);
			}

			schema.Items.Add (rootModule.CreateSchema ());
			return schema;
		}

		public static string Serialize (Configuration config)
		{
			var settings = new XmlWriterSettings ();
			settings.ConformanceLevel = ConformanceLevel.Document;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;

			var output = new StringBuilder ();
			using (var writer = XmlTextWriter.Create (output, settings)) {
				rootModule.Serialize (writer, config);
			}

			return output.ToString ();
		}

		public static Configuration Deserialize (XmlSchema schema, string xml)
		{
			var settings = new XmlReaderSettings ();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas.Add (schema);
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;

			var reader = XmlReader.Create (new StringReader (xml), settings);

			var config = new Configuration ();
			rootModule.Deserialize (reader, config);
			return config;
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
		
		internal static T DeserializeValue<T> (string value)
		{
			return (T)DeserializeValue (typeof (T), value);
		}

		internal static object DeserializeValue (Type type, string value)
		{
			if (type == typeof(bool))
				return bool.Parse (value);
			else if (type == typeof(int))
				return int.Parse (value);
			else if (type == typeof(long))
				return long.Parse (value);
			else if (type == typeof(TimeSpan))
				return TimeSpan.Parse (value);
			else if (type == typeof(string))
				return value;
			else if (type.IsEnum)
				return Enum.Parse (type, value);
			else if (type == typeof(Encoding))
				return Encoding.GetEncoding (value);
			else
				throw new InvalidOperationException ();
		}

		internal static XmlSchemaSimpleType CreateEnumerationType (Type type)
		{
			var simple = new XmlSchemaSimpleType ();
			
			var baseType = XmlSchemaSimpleType.GetBuiltInSimpleType (XmlTypeCode.String);
			
			var restriction = new XmlSchemaSimpleTypeRestriction ();
			restriction.BaseTypeName = baseType.QualifiedName;
			
			simple.Content = restriction;
			
			foreach (var member in Enum.GetNames (type)) {
				var facet = new XmlSchemaEnumerationFacet ();
				facet.Value = member;
				restriction.Facets.Add (facet);
			}
			
			return simple;
		}
		
	}
}

