//
// Utils.cs
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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using SysConfig = System.Configuration;

namespace WCF.Config {

	public static class Utils {

		public static void Dump (string filename)
		{
			if (!File.Exists (filename)) {
				Console.WriteLine ("ERROR: File does not exist!");
				return;
			}
			using (var reader = new StreamReader (filename)) {
				Console.WriteLine (reader.ReadToEnd ());
				Console.WriteLine ();
				Console.WriteLine ();
			}
		}
		
		public static void PrettyPrintXML (string filename)
		{
			var doc = new XmlDocument ();
			doc.Load (filename);
			
			using (var writer = new XmlTextWriter (new StreamWriter (filename))) {
				writer.Formatting = Formatting.Indented;
				doc.WriteTo (writer);
			}
		}

		public static void ValidateSchema (string xmlFilename, string schemaFilename)
		{
			var schema = new XmlSchemaSet ();
			schema.Add (Generator.Namespace, schemaFilename);
			
			var settings = new XmlReaderSettings ();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas = schema;
			
			var reader = XmlReader.Create (xmlFilename, settings);
			while (reader.Read ())
				;
		}

		static T CreateConfigElement<T> (Binding binding)
			where T : StandardBindingElement, new()
		{
			var element = new T ();
			var bf = BindingFlags.Instance | BindingFlags.NonPublic;
			var initFrom = typeof (T).GetMethod ("InitializeFrom", bf);
			initFrom.Invoke (element, new object[] { binding });
			return element;
		}

		static void HandleExtension<T> (CustomBindingElement custom, BindingElement element)
			where T : BindingElementExtensionElement, new()
		{
			var ext = new T ();
			if (!ext.BindingElementType.Equals (element.GetType ()))
				return;
			var bf = BindingFlags.Instance | BindingFlags.NonPublic;
			var initFrom = typeof (T).GetMethod ("InitializeFrom", bf);
			initFrom.Invoke (ext, new object[] { element });
			custom.Add (ext);
		}

		static CustomBindingElement CreateCustomElement (CustomBinding binding)
		{
			var custom = new CustomBindingElement ();
			custom.Name = binding.Name;

			foreach (var element in binding.Elements) {
				HandleExtension<TextMessageEncodingElement> (custom, element);
			}

			return custom;
		}

		public static void SaveConfig (Configuration root, string name)
		{
			if (File.Exists (name))
				File.Delete (name);

			var map = new SysConfig.ExeConfigurationFileMap ();
			map.ExeConfigFilename = name;
			
			var config = SysConfig.ConfigurationManager.OpenMappedExeConfiguration (
				map, SysConfig.ConfigurationUserLevel.None);

			var bindings = BindingsSection.GetSection (config);

			foreach (var binding in root.Bindings) {
				var http = binding as BasicHttpBinding;
				if (http != null) {
					var httpElement = CreateConfigElement<BasicHttpBindingElement> (http);
					bindings.BasicHttpBinding.Bindings.Add (httpElement);
				}

				var custom = binding as CustomBinding;
				if (custom != null) {
					var customElement = CreateCustomElement (custom);
					bindings.CustomBinding.Bindings.Add (customElement);
				}
			}
			
			config.Save (SysConfig.ConfigurationSaveMode.Modified);
			
			Utils.Dump (name);
		}
	}
}

