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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using Mono.ServiceModel.Configuration;
using Mono.ServiceModel.Configuration.Modules;
using WCF.Config.Test;
using Mono.Options;

namespace WCF.Config.Helper {

	class MainClass {

		enum Mode {
			Default,
			Xml,
			Validate,
			Download,
			Service
		}

		public static void Main (string[] args)
		{
			Uri service = null;
			string xml = null, xsd = null, wsdl = null, dir = null;
			var mode = Mode.Default;
			var options = new OptionSet ();
			options.Add ("mode=", m => {
				if (Enum.TryParse<Mode> (m, true, out mode))
					return;
				throw new ArgumentException ("Invalid -mode= argument.");
			});
			options.Add ("xml=", m => xml = m);
			options.Add ("xsd=", m => xsd = m);
			options.Add ("wsdl=", m => wsdl = m);
			options.Add ("service=", m => service = new Uri (m));
			options.Add ("dir=", m => dir = m);

			IList<string> extraArgs;
			try {
				extraArgs = options.Parse (args);
			} catch (Exception ex) {
				Console.Error.WriteLine ("ERROR: {0}", ex.Message);
				return;
			}

			if (xml == null)
				xml = "test.xml";
			if (xsd == null)
				xsd = Path.GetFileNameWithoutExtension (xml) + ".xsd";
			if (wsdl == null)
				wsdl = Path.GetFileNameWithoutExtension (xml) + ".wsdl";

			if (extraArgs.Count > 0) {
				Console.Error.WriteLine ("Unexpected extra arguments.");
				return;
			}

			if (dir != null) {
				xml = Path.Combine (dir, xml);
				xsd = Path.Combine (dir, xsd);
				wsdl = Path.Combine (dir, wsdl);
			}

			switch (mode) {
			case Mode.Xml:
				BindingTests.Run (xml, xsd);
				TestUtils.Deserialize (xml, xsd);
				break;

			case Mode.Validate:
				Utils.ValidateSchema (xml, xsd);
				break;

			case Mode.Default:
				if (File.Exists (xml))
					goto case Mode.Validate;
				else
					goto case Mode.Xml;

			case Mode.Download:
				if (service == null) {
					Console.Error.WriteLine ("Missing -service=<url> argument.");
					return;
				}

				TestUtils.GenerateFromWsdl (service, wsdl, xml, xsd);
				break;

			case Mode.Service:
				TestService (xml, xsd);
				break;

			default:
				throw new InvalidOperationException ();
			}
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

		static void TestService (string xmlFilename, string xsdFilename)
		{
			ConfigurationHost.Install (xmlFilename, xsdFilename);
			var client = new MyService.MyServiceClient ();
			var hello = client.Hello ();
			Console.WriteLine ("Got response from service: {0}", hello);
		}
	}
}
