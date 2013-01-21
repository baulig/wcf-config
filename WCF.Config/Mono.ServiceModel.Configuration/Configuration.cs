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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Mono.ServiceModel.Configuration {

	internal class Configuration {

		bool deserialized;
		Collection<Binding> bindings = new Collection<Binding> ();
		Collection<Endpoint> endpoints = new Collection<Endpoint> ();

		internal Collection<Binding> Bindings {
			get { return bindings; }
		}

		internal Collection<Endpoint> Endpoints {
			get { return endpoints; }
		}

		public void AddBinding (Binding binding)
		{
			if (deserialized)
				throw new InvalidOperationException ();

			if (!bindings.Contains (binding))
				bindings.Add (binding);
		}

		public void AddEndpoint (ServiceEndpoint sep)
		{
			if (deserialized)
				throw new InvalidOperationException ();

			AddBinding (sep.Binding);

			Console.WriteLine ("ADD ENDPOINT: {0} {1}", sep.Binding, sep.Address);

			if (sep.Address == null || sep.Address.Uri.AbsoluteUri == null)
				throw new InvalidOperationException ();

			var endpoint = new Endpoint ();
			endpoint.Name = sep.Name;
			endpoint.Address = sep.Address.Uri.ToString ();
			endpoint.Contract = sep.Contract.Name;
			endpoint.Binding = sep.Binding.Name;
			endpoint.ServiceEndpoint = sep;
			endpoints.Add (endpoint);
		}

		public IList<ServiceEndpoint> GetEndpoints (ContractDescription contract)
		{
			var list = new List<ServiceEndpoint> ();
			foreach (var endpoint in endpoints) {
				if (endpoint.ServiceEndpoint != null) {
					if (endpoint.ServiceEndpoint.Contract == contract)
						list.Add (endpoint.ServiceEndpoint);
					continue;
				}

				// var name = contract.ConfigurationName ?? contract.Name;
				var name = contract.Name;
				if (!endpoint.Contract.Equals (name))
					continue;

				var binding = Bindings.First (b => b.Name.Equals (endpoint.Binding));
				endpoint.ServiceEndpoint = new ServiceEndpoint (contract);
				endpoint.ServiceEndpoint.Binding = binding;
				endpoint.ServiceEndpoint.Address = new EndpointAddress (endpoint.Address);
				list.Add (endpoint.ServiceEndpoint);
			}

			return list.AsReadOnly ();
		}

		public void Serialize (Context context, XmlWriter writer)
		{
			Generator.RootModule.Serialize (context, writer, this);
		}

		public Configuration ()
		{
		}

		public Configuration (Context context, string xmlFilename, string schemaFilename)
		{
			deserialized = true;

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
				Generator.RootModule.Deserialize (context, xml, this);
			}
		}

	}
}

