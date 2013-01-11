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

namespace WCF.Config.MonoTouch.Test {

	using Mono.System.ServiceModel.Configuration;
	using Mono.System.ServiceModel.Configuration.Modules;
	
	public static class Test {

		public static void Run (string xmlFilename, string xsdFilename)
		{
			if (File.Exists (xmlFilename) && File.Exists (xsdFilename)) {
				Utils.ValidateSchema (xmlFilename, xsdFilename);
				return;
			}
			
			var http = new BasicHttpBinding ();
			http.OpenTimeout = TimeSpan.FromHours (3);
			http.MaxBufferSize = 8192;
			http.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;
			http.AllowCookies = true;
			http.Security.Mode = BasicHttpSecurityMode.Transport;
			http.TransferMode = TransferMode.StreamedRequest;

			var custom = new CustomBinding ();
			custom.Name = "myCustomBinding";
			var text = new TextMessageEncodingBindingElement ();
			text.MessageVersion = MessageVersion.Soap12WSAddressingAugust2004;
			custom.Elements.Add (text);
			custom.Elements.Add (new HttpTransportBindingElement ());
			
			var root = new Configuration ();
			root.Bindings.Add (http);
			root.Bindings.Add (custom);
			
			var endpoint = new Endpoint ();
			endpoint.Name = "myEndpoint";
			endpoint.Contract = "myContract";
			endpoint.Binding = "myBinding";
			root.Endpoints.Add (endpoint);
			// root.Endpoints.Add (endpoint);

			Generator.Write (xmlFilename, xsdFilename, root);
			
			Utils.Dump (xsdFilename);
			Utils.Dump (xmlFilename);
			
			Utils.ValidateSchema (xmlFilename, xsdFilename);
		}

	}
}

