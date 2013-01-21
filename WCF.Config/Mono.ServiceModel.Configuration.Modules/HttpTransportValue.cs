//
// HttpTransportValue.cs
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
using System.ServiceModel.Channels;

namespace Mono.ServiceModel.Configuration.Modules {

	public class HttpTransportValue : Value<HttpTransportBindingElement> {

		public HttpTransportValue ()
		{
			AddAttribute (
				"allowCookies", i => i.AllowCookies, (i,v) => i.AllowCookies = v);
			AddAttribute (
				"authenticationSchema", i => i.AuthenticationScheme,
				(i,v) => i.AuthenticationScheme = v);
			AddAttribute (
				"bypassProxyOnLocal", i => i.BypassProxyOnLocal,
				(i,v) => i.BypassProxyOnLocal = v);
			AddAttribute (
				"hostNameComparisonMode", i => i.HostNameComparisonMode,
				(i,v) => i.HostNameComparisonMode = v);
			AddAttribute (
				"keepAliveEnabled", i => i.KeepAliveEnabled,
				(i,v) => i.KeepAliveEnabled = v);
			AddAttribute (
				"maxBufferSize", i => i.MaxBufferSize,
				(i,v) => i.MaxBufferSize = v).SetMinMax ("1", int.MaxValue.ToString ());
#if !MOBILE
			AddAttribute (
				"decompressionEnabled", i => i.DecompressionEnabled,
				(i,v) => i.DecompressionEnabled = v);
#endif
			
			AddAttribute (
				"proxyAddress", i => i.ProxyAddress, (i,v) => i.ProxyAddress = v);
			AddAttribute (
				"proxyAuthenticationScheme", i => i.ProxyAuthenticationScheme,
				(i,v) => i.ProxyAuthenticationScheme = v);
			AddAttribute (
				"useDefaultWebProxy", i => i.UseDefaultWebProxy,
				(i,v) => i.UseDefaultWebProxy = v);
			AddAttribute (
				"realm", i => i.Realm, (i,v) => i.Realm = v);
			AddAttribute (
				"transferMode", i => i.TransferMode, (i,v) => i.TransferMode = v);
			AddAttribute (
				"unsafeConnectionNtlmAuthentication", i => i.UnsafeConnectionNtlmAuthentication,
				(i,v) => i.UnsafeConnectionNtlmAuthentication = v);
		}

	}
}

