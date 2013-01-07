//
// BindingModule.cs
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCF.Config {

	public class BindingsModule : Module {
		static IList<Module> children;

		static BindingsModule ()
		{
			var list = new List<Module> ();
			list.Add (new BasicHttpBindingModule ());
			children = list.AsReadOnly ();
		}

		public override string Name {
			get { return "bindings"; }
		}

		public override bool IsSupported (object instance)
		{
			return instance is IList<Binding>;
		}

		public override Value GetValue (object instance)
		{
			return new BindingList (this, (IList<Binding>)instance);
		}

		protected override void CreateSchema (XmlSchemaElement element)
		{
			var complex = new XmlSchemaComplexType ();
			var all = new XmlSchemaAll ();
			foreach (var child in Children) {
				all.Items.Add (child.CreateSchema ());
			}
			complex.Particle = all;
			element.SchemaType = complex;
		}

		public override bool HasChildren {
			get { return true; }
		}

		public override IList<Module> Children {
			get { return children; }
		}
	}
}

