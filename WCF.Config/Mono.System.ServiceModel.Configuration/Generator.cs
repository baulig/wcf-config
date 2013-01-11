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

	public static class Generator {
		static readonly XmlSchema schema;
		static readonly RootModule rootModule;
		static readonly Dictionary<Type, Module> moduleMap;

		public const string Prefix = null;
		public const string Namespace = "https://github.com/baulig/wcf-config/schema";

		static Generator ()
		{
			moduleMap = new Dictionary<Type, Module> ();
			rootModule = new RootModule ();
			schema = SchemaTypeMap.CreateSchema (rootModule);
		}

		public static void Write (string xmlFilename, string xsdFilename, Configuration config)
		{
			var settings = new XmlWriterSettings ();
			settings.ConformanceLevel = ConformanceLevel.Document;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.CloseOutput = false;

			using (var stream = new StreamWriter (Utils.GetFilename (xsdFilename))) {
				using (var writer = XmlWriter.Create (stream, settings)) {
					Schema.Write (writer);
					writer.WriteEndDocument ();
				}
				stream.WriteLine ();
			}

			using (var stream = new StreamWriter (Utils.GetFilename (xmlFilename))) {
				using (var writer = XmlWriter.Create (stream, settings)) {
					config.Serialize (writer);
					writer.WriteEndDocument ();
				}
				stream.WriteLine ();
			}
		}

		internal static Module RootModule {
			get { return rootModule; }
		}

		public static XmlSchema Schema {
			get { return schema; }
		}

		internal static void RegisterModule (Module module)
		{
			moduleMap.Add (module.GetType (), module);
		}

		public static T GetModule<T> ()
			where T : Module, new()
		{
			if (!moduleMap.ContainsKey (typeof(T))) {
				// Module's ctor calls RegisterModule().
				return new T ();
			} else {
				return (T)moduleMap [typeof(T)];
			}
		}

#if !MOBILE
		public static T GetModule<T,V> ()
			where T : Module<V>, new()
			where V : class, new()
		{
			return GetModule<T> ();
		}
#endif

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

		internal static XmlTypeCode GetTypeCode (Type type)
		{
			if (type == typeof (string))
				return XmlTypeCode.String;
			else if (type == typeof (bool))
				return XmlTypeCode.Boolean;
			else if (type == typeof (int))
				return XmlTypeCode.Int;
			else if (type == typeof (long))
				return XmlTypeCode.Long;
			else if (type == typeof (double))
				return XmlTypeCode.Double;
			else if (type == typeof (TimeSpan))
				return XmlTypeCode.Time;
			else if (type == typeof (Uri))
				return XmlTypeCode.AnyUri;
			else if (type == typeof (Encoding))
				return XmlTypeCode.String;
			else
				throw new ArgumentException (string.Format (
					"Unknown attribute type `{0}'", type));
		}
		
		internal static object DeserializeValue (Type type, string value)
		{
			if (type == typeof(bool))
				return bool.Parse (value);
			else if (type == typeof(int))
				return int.Parse (value);
			else if (type == typeof(long))
				return long.Parse (value);
			else if (type == typeof(double))
				return double.Parse (value);
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

		internal static XmlQualifiedName GetEnumerationType (Type type, SchemaTypeMap map)
		{
			if (map.IsRegistered (type))
				return map.LookupTypeName (type);

			var simple = new XmlSchemaSimpleType ();

			simple.Name = "enum" + type.Name;
			
			var baseType = XmlSchemaSimpleType.GetBuiltInSimpleType (XmlTypeCode.String);
			
			var restriction = new XmlSchemaSimpleTypeRestriction ();
			restriction.BaseTypeName = baseType.QualifiedName;
			
			simple.Content = restriction;
			
			foreach (var member in Enum.GetNames (type)) {
				var facet = new XmlSchemaEnumerationFacet ();
				facet.Value = member;
				restriction.Facets.Add (facet);
			}

			return map.RegisterType (type, simple);
		}
		
	}
}

