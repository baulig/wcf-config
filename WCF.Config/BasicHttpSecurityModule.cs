//
// BasicHttpSecurityModule.cs
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

namespace WCF.Config {

	public class BasicHttpSecurityModule : ValueModule<BasicHttpSecurity> {

		public override string Name {
			get { return "security"; }
		}

		protected override void GetAttributes (AttributeList<BasicHttpSecurity> list)
		{
			list.Add ("mode", i => i.Mode, (i,v) => i.Mode = v);
			base.GetAttributes (list);
		}

		protected override void Populate ()
		{
			AddElement<HttpTransportSecurity, HttpTransportSecurityModule> (i => i.Transport);
			base.Populate ();
		}

#if FIXME
			(i,d) => {
				if (d || (i.Transport.ClientCredentialType != HttpClientCredentialType.None) ||
				    (i.Transport.ProxyCredentialType != HttpProxyCredentialType.None) ||
				    !string.IsNullOrEmpty (i.Transport.Realm))
					return i.Transport;
				return null;
			});
#endif

	}
}
