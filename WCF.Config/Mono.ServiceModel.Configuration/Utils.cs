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
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
#if !MOBILE
using WS = System.Web.Services.Description;
#endif

namespace Mono.ServiceModel.Configuration {

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
			return Path.Combine (documents, Path.GetFileName (filename));
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

		public static void DownloadXml (Uri uri, string filename)
		{
			var wc = new WebClient ();
			using (var stream = wc.OpenRead (uri)) {
				var reader = new XmlTextReader (stream);
				using (var writer = new XmlTextWriter (GetFilename (filename), Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					while (reader.Read ())
						WriteShallowNode (reader, writer);
				}
			}
		}
		
		// From http://blogs.msdn.com/b/mfussell/archive/2005/02/12/371546.aspx
		static void WriteShallowNode( XmlReader reader, XmlWriter writer )
		{
			if ( reader == null )
			{
				throw new ArgumentNullException("reader");
			}
			if ( writer == null )
			{
				throw new ArgumentNullException("writer");
			}
			
			switch ( reader.NodeType )
			{
			case XmlNodeType.Element:
				writer.WriteStartElement( reader.Prefix, reader.LocalName, reader.NamespaceURI );
				writer.WriteAttributes( reader, true );
				if ( reader.IsEmptyElement )
				{
					writer.WriteEndElement();
				}
				break;
			case XmlNodeType.Text:
				writer.WriteString( reader.Value );
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				writer.WriteWhitespace(reader.Value);
				break;
			case XmlNodeType.CDATA:
				writer.WriteCData( reader.Value );
				break;
			case XmlNodeType.EntityReference:
				writer.WriteEntityRef(reader.Name);
				break;
			case XmlNodeType.XmlDeclaration:
			case XmlNodeType.ProcessingInstruction:
				writer.WriteProcessingInstruction( reader.Name, reader.Value );
				break;
			case XmlNodeType.DocumentType:
				writer.WriteDocType( reader.Name, reader.GetAttribute( "PUBLIC" ), reader.GetAttribute( "SYSTEM" ), reader.Value );
				break;
			case XmlNodeType.Comment:
				writer.WriteComment( reader.Value );
				break;
			case XmlNodeType.EndElement:
				writer.WriteFullEndElement();
				break;
			}
		}

#if !MOBILE
		public static MetadataSet LoadMetadata (Uri uri, string filename)
		{
			var path = GetFilename (filename);
			if (!File.Exists (path)) {
				Console.WriteLine ("Downloading service metadata ...");
				DownloadXml (uri, filename);
				Console.WriteLine ("Downloaded service metadata into {0}.", filename);
			} else {
				Console.WriteLine ("Loading cached service metadata from {0}.", filename);
			}
			
			using (var stream = new StreamReader (path)) {
				var doc = new MetadataSet ();
				var service = WS.ServiceDescription.Read (stream);
				var sect = new MetadataSection (
					"http://schemas.xmlsoap.org/wsdl/", "http://tempuri.org/", service);
				doc.MetadataSections.Add (sect);
				return doc;
			}
		}
#endif

	}
}

