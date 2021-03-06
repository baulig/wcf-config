//
// DebugUtils.cs
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
using System.ServiceModel.Channels;

namespace Mono.ServiceModel.Configuration {

	static class DebugUtils {
		public static void Dump (Configuration config)
		{
			foreach (var binding in config.Bindings) {
				Console.WriteLine ("BINDING: {0}", binding);
				var http = binding as BasicHttpBinding;
				if (http != null)
					Dump (http);
#if !MOBILE || MOBILE_BAULIG
				var https = binding as BasicHttpsBinding;
				if (https != null)
					Dump (https);
#endif
				var custom = binding as CustomBinding;
				if (custom != null)
					Dump (custom);
			}
			foreach (var endpoint in config.Endpoints) {
				Console.WriteLine ("ENDPOINT: {0}", endpoint);
			}
		}
		
		public static void Dump (BasicHttpBinding binding)
		{
			Console.WriteLine ("HTTP: {0} {1} {2} {3}",
			                   binding.Name, binding.OpenTimeout, binding.Security.Mode,
			                   binding.TransferMode);
		}
		
#if !MOBILE || MOBILE_BAULIG
		public static void Dump (BasicHttpsBinding binding)
		{
			Console.WriteLine ("HTTPS: {0} {1} {2} {3}",
			                   binding.Name, binding.OpenTimeout, binding.Security.Mode,
			                   binding.TransferMode);
		}
#endif
		
		public static void Dump (CustomBinding binding)
		{
			Console.WriteLine ("CUSTOM: {0}", binding.Name);
			
			foreach (var element in binding.Elements)
				Console.WriteLine ("ELEMENT: {0}", element);
		}
	}
}

