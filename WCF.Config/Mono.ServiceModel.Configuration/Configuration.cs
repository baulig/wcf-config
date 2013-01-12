//
// Configuration.cs
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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Mono.ServiceModel.Configuration {

	public class Configuration {

		Collection<Binding> bindings = new Collection<Binding> ();
		Collection<Endpoint> endpoints = new Collection<Endpoint> ();

		public Collection<Binding> Bindings {
			get { return bindings; }
		}

		public Collection<Endpoint> Endpoints {
			get { return endpoints; }
		}

		public void AddEndpoint (ServiceEndpoint sep)
		{
			if (!bindings.Contains (sep.Binding))
				bindings.Add (sep.Binding);

			var endpoint = new Endpoint ();
			endpoint.Name = sep.Name;
			endpoint.Contract = sep.Contract.Name;
			endpoint.Binding = sep.Binding.Name;
			endpoints.Add (endpoint);
		}

		public string Serialize ()
		{
			var settings = new XmlWriterSettings ();
			settings.ConformanceLevel = ConformanceLevel.Document;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;

			var output = new StringBuilder ();
			using (var writer = XmlTextWriter.Create (output, settings)) {
				Serialize (writer);
			}
			
			return output.ToString ();
		}

		public void Serialize (XmlWriter writer)
		{
			Generator.RootModule.Serialize (writer, this);
		}

		public Configuration ()
		{
		}

		public void Deserialize (string xmlFilename, string schemaFilename)
		{
			var schema = new XmlSchemaSet ();
			schema.Add (Generator.Namespace, Utils.GetFilename (schemaFilename));
			schema.Compile ();
				
			var settings = new XmlReaderSettings {
				ValidationType = ValidationType.Schema,
				ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
				XmlSchemaValidationFlags.ProcessSchemaLocation |
				XmlSchemaValidationFlags.ReportValidationWarnings |
				XmlSchemaValidationFlags.ProcessIdentityConstraints,
				IgnoreComments = true, IgnoreWhitespace = true
			};
				
			settings.Schemas.Add (schema);

			using (var xml = XmlReader.Create (Utils.GetFilename (xmlFilename), settings)) {
				Generator.RootModule.Deserialize (xml, this);
			}
		}

	}
}

