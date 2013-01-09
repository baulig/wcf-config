//
// BasicHttpBindingModule.cs
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
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.ServiceModel;

namespace WCF.Config {

	public class BasicHttpBindingModule : HttpBindingBaseModule<BasicHttpBinding> {

		static readonly BasicHttpSecurityModule securityModule;
		static IList<Module> children;

		static BasicHttpBindingModule ()
		{
			securityModule = new BasicHttpSecurityModule ();
			var list = new List<Module> ();
			list.Add (securityModule);
			children = list.AsReadOnly ();
		}

		public override string Name {
			get { return "basicHttpBinding"; }
		}

		public override bool HasChildren {
			get { return true; }
		}

		public override IList<Module> Children {
			get { return children; }
		}

		protected override void GetAttributes (AttributeList<BasicHttpBinding> list)
		{
			list.Add ("messageEncoding", i => i.MessageEncoding, (i,v) => i.MessageEncoding = v);
			base.GetAttributes (list);
		}

		public override Value<BasicHttpBinding> GetValue (BasicHttpBinding instance)
		{
			return new ValueImpl (this, instance);
		}

		class ValueImpl : Value<BasicHttpBinding> {
			public ValueImpl (BasicHttpBindingModule module, BasicHttpBinding instance)
				: base (module, instance)
			{ }

			protected override void GetChildren (List<Value> list)
			{
				list.Add (securityModule.GetValue (Instance.Security));
				base.GetChildren (list);
			}
		}

	}
}
