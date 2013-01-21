//
// NetTcpSecurityModule.cs
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
#if !MOBILE
using System;
using System.ServiceModel;

namespace Mono.ServiceModel.Configuration.Modules {
	
	class NetTcpSecurityModule : ValueModule<NetTcpSecurity> {
		
		public override string Name {
			get { return "netTcpSecurity"; }
		}
		
		protected override void Populate ()
		{
			AddAttribute ("mode", i => i.Mode, (i,v) => i.Mode = v);
			AddElement<TcpTransportSecurityModule, TcpTransportSecurity> (i => i.Transport);
			base.Populate ();
		}
		
		public override bool IsDefault (NetTcpSecurity instance)
		{
			if (instance.Mode != SecurityMode.None)
				return false;
			var transport = Generator.GetModule<TcpTransportSecurityModule, TcpTransportSecurity> ();
			return transport.IsDefault (instance.Transport);
		}

		protected override bool IsSupported (Context context, NetTcpSecurity instance)
		{
			if (instance.Mode != SecurityMode.None) {
				context.AddError ("SecurityMode '{0}' is not supported.", instance.Mode);
				return false;
			}
			return base.IsSupported (context, instance);
		}
		
	}
}
#endif
