//
// TestUtils.cs
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
using System.ServiceModel;
using System.ServiceModel.Description;

using Mono.ServiceModel.Configuration;

namespace WCF.Config.Test {

	public static class TestUtils {
#if !MOBILE
		public static void GenerateFromWsdl (Uri uri, string wsdlFilename,
		                                     string xmlFilename, string xsdFilename)
		{
			var doc = Utils.LoadMetadata (uri, wsdlFilename);
			var importer = new WsdlImporter (doc);
			var endpoints = importer.ImportAllEndpoints ();
			
			var config = new Configuration ();
			foreach (var endpoint in endpoints)
				config.AddEndpoint (endpoint);
			
			Generator.Write (xmlFilename, xsdFilename, config);
		}
#endif

		public static void Deserialize (string xmlFilename, string xsdFilename)
		{
			var config = new Configuration (xmlFilename, xsdFilename);
			Console.WriteLine ("READ CONFIG FROM XML");
			
			DebugUtils.Dump (config);
		}
	}
}

