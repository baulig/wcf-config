//
// Test.cs
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
#if FIXME
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using Mono.ServiceModel.Configuration;

namespace WCF.Config.Test {

	public static class BindingTests {

		public static void Run ()
		{
			Run ("test.xml", "test.xsd");
		}

		public static void Run (string xmlFilename, string xsdFilename)
		{
			var http = new BasicHttpBinding ();
			http.OpenTimeout = TimeSpan.FromHours (3);
			http.MaxBufferSize = 8192;
			http.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;
			http.AllowCookies = true;
			http.Security.Mode = BasicHttpSecurityMode.Transport;
			http.TransferMode = TransferMode.StreamedRequest;

#if !MOBILE || MOBILE_BAULIG
			var https = new BasicHttpsBinding ();
			https.MaxBufferSize = 32768;
#endif

#if !MOBILE_FIXME
			var netTcp = new NetTcpBinding ();
#endif
			
			var custom = new CustomBinding ();
			custom.Name = "myCustomBinding";
			var text = new TextMessageEncodingBindingElement ();
			text.MessageVersion = MessageVersion.Soap12WSAddressingAugust2004;
			custom.Elements.Add (text);
			custom.Elements.Add (new HttpTransportBindingElement ());
			
			var root = new Configuration ();
			root.AddBinding (http);
#if !MOBILE || MOBILE_BAULIG
			root.AddBinding (https);
#endif
#if !MOBILE_FIXME
			root.AddBinding (netTcp);
#endif
			root.AddBinding (custom);
			
			var contract = new ContractDescription ("MyContract");
			var endpointUri = "custom://localhost:8888/MyService";
			var endpointAddress = new EndpointAddress (endpointUri);

			var endpoint = new ServiceEndpoint (contract, custom, endpointAddress);
			
			root.AddEndpoint (endpoint);
			
			Generator.Write (xmlFilename, xsdFilename, root);
			
			Utils.Dump (xsdFilename);
			Utils.Dump (xmlFilename);
			
			Utils.ValidateSchema (xmlFilename, xsdFilename);
		}
	}
}

#endif
