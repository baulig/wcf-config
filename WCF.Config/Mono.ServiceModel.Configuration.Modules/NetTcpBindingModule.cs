//
// NetTcpBindingModule.cs
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

namespace Mono.ServiceModel.Configuration.Modules {
	
	public class NetTcpBindingModule : ValueModule<NetTcpBinding> {
		
		public override string Name {
			get { return "netTcpBinding"; }
		}
		
		protected override void Populate ()
		{
			Implement<BindingValue> ();
#if !MOBILE || MOBILE_BAULIG
			AddElement<NetTcpSecurityModule, NetTcpSecurity> (i => i.Security);
#endif

			AddAttribute (
				"hostNameComparisonMode", i => i.HostNameComparisonMode,
				(i,v) => i.HostNameComparisonMode = v);
			AddAttribute (
				"listenBacklog", i => i.ListenBacklog,
				(i,v) => i.ListenBacklog = v);
			AddAttribute (
				"maxBufferPoolSize", i => i.MaxBufferPoolSize,
				(i,v) => i.MaxBufferPoolSize = v).SetMinMax ("0", "9223372036854775807");
			AddAttribute (
				"maxBufferSize", i => i.MaxBufferSize,
				(i,v) => i.MaxBufferSize = v).SetMinMax ("1", int.MaxValue.ToString ());
			AddAttribute (
				"maxConnections", i => i.MaxConnections,
				(i,v) => i.MaxConnections = v).SetMinMax ("1", int.MaxValue.ToString ());
			AddAttribute (
				"maxReceiveMessageSize", i => i.MaxReceivedMessageSize,
				(i,v) => i.MaxReceivedMessageSize = v).SetMinMax ("0", "9223372036854775807");
			AddAttribute (
				"portSharingEnabled", i => i.PortSharingEnabled,
				(i,v) => i.PortSharingEnabled = v);
			AddAttribute ("transferMode", i => i.TransferMode, (i,v) => i.TransferMode = v);

			base.Populate ();
		}
	}

}
