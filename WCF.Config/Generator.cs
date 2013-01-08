//
// Generator.cs
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
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCF.Config {

	public class Generator {
		static readonly IList<Module> modules;
		static readonly BindingsModule bindingsModule;

		public const string Namespace = "https://github.com/baulig/wcf-config/schema";

		static Generator ()
		{
			var list = new List<Module> ();
			bindingsModule = new BindingsModule ();
			list.Add (bindingsModule);
			modules = list.AsReadOnly ();
		}

		public static XmlSchema CreateSchema ()
		{
			var schema = new XmlSchema ();
			schema.TargetNamespace = Namespace;
			foreach (var module in modules) {
				schema.Items.Add (module.CreateSchema ());
			}
			return schema;
		}

		public static string Serialize (params Binding[] bindings)
		{
			var settings = new XmlWriterSettings ();
			settings.ConformanceLevel = ConformanceLevel.Document;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;

			var output = new StringBuilder ();
			using (var writer = XmlTextWriter.Create (output, settings)) {
				bindingsModule.Serialize (writer, bindings);
			}

			return output.ToString ();
		}
	}
}
