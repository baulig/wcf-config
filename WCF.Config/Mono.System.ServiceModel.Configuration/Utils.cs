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
using System.Xml.XPath;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Mono.System.ServiceModel.Configuration {

	public static class Utils {

		public static void Dump (string filename)
		{
			filename = Utils.GetFilename (filename);
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

		public static string GetFilename (string filename)
		{
#if MOBILE
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			return Path.Combine (documents, filename);
#else
			return filename;
#endif
		}

		public static void ValidateSchema (string xmlFilename, string schemaFilename)
		{
			var schema = new XmlSchemaSet ();
			schema.Add (Generator.Namespace, GetFilename (schemaFilename));
			schema.Compile ();

			var settings = new XmlReaderSettings {
				ValidationType = ValidationType.Schema,
				ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
				XmlSchemaValidationFlags.ProcessSchemaLocation |
				XmlSchemaValidationFlags.ReportValidationWarnings |
				XmlSchemaValidationFlags.ProcessIdentityConstraints,
			};
			
			settings.Schemas.Add (schema);

			var reader = XmlReader.Create (GetFilename (xmlFilename), settings);
			while (reader.Read ())
				;

			Console.WriteLine ("Document {0} successfully validated against schema {1}.",
			                   xmlFilename, schemaFilename);
			Console.WriteLine ();
		}
	}
}

