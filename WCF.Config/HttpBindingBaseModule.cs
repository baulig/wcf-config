//
// HttpBindingBaseModule.cs
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
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.ServiceModel;

namespace WCF.Config {
	
	public abstract class HttpBindingBaseModule<T> : BindingModule<T>
		where T: HttpBindingBase, new()
	{
		protected override void GetAttributes (AttributeList<T> list)
		{
			list.Add ("allowCookies", i => i.AllowCookies, (i,v) => i.AllowCookies = v);
			list.Add (
				"bypassProxyOnLocal", i => i.BypassProxyOnLocal,
				(i,v) => i.BypassProxyOnLocal = v);
			list.Add (
				"hostNameComparisonMode", i => i.HostNameComparisonMode,
				(i,v) => i.HostNameComparisonMode = v);
			list.Add (
				"maxBufferPoolSize", i => i.MaxBufferPoolSize,
				(i,v) => i.MaxBufferPoolSize = v).SetMinMax ("0", "9223372036854775807");
			list.Add (
				"maxBufferSize", i => i.MaxBufferSize,
				(i,v) => i.MaxBufferSize = v).SetMinMax ("1", int.MaxValue.ToString ());
			list.Add (
				"maxReceivedMessageSize", i => i.MaxReceivedMessageSize,
				(i,v) => i.MaxReceivedMessageSize = v).SetMinMax ("1", "9223372036854775807");
			list.Add (
				"useDefaultWebProxy", i => i.UseDefaultWebProxy,
				(i,v) => i.UseDefaultWebProxy = v);
			list.Add ("proxyAddress", i => i.ProxyAddress, (i,v) => i.ProxyAddress = v);
			list.Add ("textEncoding", i => i.TextEncoding, (i,v) => i.TextEncoding = v);
			list.Add ("transferMode", i => i.TransferMode, (i,v) => i.TransferMode = v);

			base.GetAttributes (list);
		}

		public override Value<T> GetValue (T instance)
		{
			return new _Value (this, instance);
		}
		
		class _Value : Value<T> {
			
			public _Value (HttpBindingBaseModule<T> module, T binding)
				: base (module, binding)
			{
			}
			
		}
	}
}

