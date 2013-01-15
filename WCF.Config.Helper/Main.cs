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
using System.Net;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using Mono.ServiceModel.Configuration;
using Mono.ServiceModel.Configuration.Modules;
using WCF.Config.Test;

namespace WCF.Config.Helper {

	class MainClass {

		public static void Main (string[] args)
		{
			Run ("test.xml", "test.xsd");
			if (!File.Exists ("config.wsdl")) {
				var uri = new Uri ("http://provcon-faust/TestWCF/Service/MyService.svc?singleWsdl");
				TestUtils.GenerateFromWsdl (uri, "config.wsdl", "config.xml", "config.xsd");
			}
			TestService ();
		}

		static void Run (string xmlFilename, string xsdFilename)
		{
			if (File.Exists (xmlFilename) && File.Exists (xsdFilename)) {
				Utils.ValidateSchema (xmlFilename, xsdFilename);
			} else {
				BindingTests.Run (xmlFilename, xsdFilename);
			}

			TestUtils.Deserialize (xmlFilename, xsdFilename);
		}

		static void TestService ()
		{
			ConfigurationHost.Install ();
			WebRequest.DefaultWebProxy = new WebProxy ("http://192.168.16.104:3128");
			var client = new MyService.MyServiceClient ("*", "http://provcon-faust/TestWCF/Service/MyService.svc");
			var hello = client.Hello ();
			Console.WriteLine ("Got response from service: {0}", hello);
		}
	}
}
