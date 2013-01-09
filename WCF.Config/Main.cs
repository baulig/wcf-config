//
// Main.cs
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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using SysConfig = System.Configuration;

namespace WCF.Config {

	class MainClass {

		static T CreateConfigElement<T> (Binding binding)
			where T : StandardBindingElement, new()
		{
			var element = new T ();
			var bf = BindingFlags.Instance | BindingFlags.NonPublic;
			var initFrom = typeof (T).GetMethod ("InitializeFrom", bf);
			initFrom.Invoke (element, new object[] { binding });
			return element;
		}

		public static void Main (string[] args)
		{
			var schema = Generator.CreateSchema ();
			schema.ElementFormDefault = XmlSchemaForm.Qualified;
			schema.AttributeFormDefault = XmlSchemaForm.Unqualified;

			schema.Write (Console.Out);
			Console.WriteLine ();
			Console.WriteLine ();

			var http = new BasicHttpBinding ();
			http.OpenTimeout = TimeSpan.FromHours (3);
			http.MaxBufferSize = 8192;
			http.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;
			http.AllowCookies = true;
			http.Security.Mode = BasicHttpSecurityMode.Transport;
			http.TransferMode = TransferMode.StreamedRequest;
			// http.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Digest;

			var netTcp = new NetTcpBinding ();

			var custom = new CustomBinding ();
			custom.Name = "myCustomBinding";
			var text = new TextMessageEncodingBindingElement ();
			text.MessageVersion = MessageVersion.Soap12WSAddressingAugust2004;
			custom.Elements.Add (text);

			var root = new Configuration ();
			root.Bindings.Add (http);
			root.Bindings.Add (http);
			root.Bindings.Add (netTcp);
			root.Bindings.Add (custom);

			var xml = Generator.Serialize (root);
			Console.WriteLine (xml);
			Console.WriteLine ();

			var sc = new XmlSchemaSet ();
			sc.Add (schema);

			var settings = new XmlReaderSettings ();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas = sc;

			var reader = XmlReader.Create (new StringReader (xml), settings);
			while (reader.Read ())
				;

			Console.WriteLine ();

			Utils.SaveConfig (root, "test.config");
			
			var deserialized = Generator.Deserialize (schema, xml);
			var test = deserialized.Bindings.OfType<CustomBinding> ().First ();
			Console.WriteLine (test.Elements.Count);
			Utils.SaveConfig (deserialized, "test2.config");
		}
	}
}
