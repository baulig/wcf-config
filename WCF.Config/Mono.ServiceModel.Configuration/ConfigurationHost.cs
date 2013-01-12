//
// ConfigurationHost.cs
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
using System.Linq;
#if !MOBILE
using System.Reflection;
#endif
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Mono.ServiceModel.Configuration {

	/*
	 * Mono-only class.
	 */
	public class ConfigurationHost
#if MOBILE
		: IMonoConfigurationHost
#endif
	{
		static ConfigurationHost instance;
		Configuration config;

		private ConfigurationHost ()
		{
#if MOBILE
			/*
			 * The Mono.ServiceModel.Configuration.IMonoConfigurationHost
			 * API is public in the mobile profile.
			 */
			MonoConfigurationHost.CustomConfigurationHost = this;
#else
			/*
			 * We use reflection here, so we don't depend on Mono
			 * additions to System.ServiceModel.dll which would cause
			 * problems when using the Mono.ServiceModel.Configuration.dll
			 * with Microsoft's runtime on Windows.
			 */
			if (Environment.OSVersion.Platform != PlatformID.Unix)
				throw new InvalidOperationException (
					"ConfigurationHost can only be used on Mono.");

			var asm = typeof (ChannelFactory).Assembly;
			var type = asm.GetType ("Mono.ServiceModel.Configuration.MonoConfigurationHost", true);
			
			var bf = BindingFlags.Static | BindingFlags.NonPublic;
			var method = type.GetMethod ("SetCustomConfigurationHandler", bf);
			if (method == null)
				throw new InvalidOperationException ();
			
			var dlgType = type.GetNestedType ("CustomConfigurationDelegate", BindingFlags.NonPublic);
			if (dlgType == null)
				throw new InvalidOperationException ();
			
			var handler = Delegate.CreateDelegate (dlgType, this, "ConfigureEndpoint");
			
			method.Invoke (null, new object[] { handler });
#endif
			
			Console.WriteLine ("Custom configuration handler installed.");

			DownloadFromMyMac ("config.xml");
			DownloadFromMyMac ("config.xsd");
			
			config = new Configuration ("config.xml", "config.xsd");

			Test.Dump (config);
			Console.WriteLine ("Configuration loaded.");
		}

		void DownloadFromMyMac (string filename)
		{
			var root = new Uri ("http://192.168.16.104/~martin/work/");
			Utils.DownloadXml (new Uri (root, filename), filename);
		}

		public bool ConfigureEndpoint (ChannelFactory factory,
		                               ref ServiceEndpoint endpoint, string endpointConfig)
		{
			Console.WriteLine ("CONFIGURE ENDPOINT: {0} {1} {2} {3} {4}",
			                   factory, endpointConfig, endpoint.Contract.Name,
			                   endpoint.Binding != null, endpoint.Address != null);

			if (endpoint.Binding != null)
				throw new NotSupportedException ();
			if (endpoint.Address == null)
				throw new NotSupportedException ();

			var address = endpoint.Address;
			var endpoints = config.GetEndpoints (endpoint.Contract);
			Console.WriteLine ("GOT ENDPOINTS: {0}", endpoints.Count);
			if (endpoints.Count != 1)
				return false;

			endpoint = endpoints [0];
			endpoint.Address = address;
			return true;
		}

		public static void Install ()
		{
			if (instance == null)
				instance = new ConfigurationHost ();
		}
	}
}

