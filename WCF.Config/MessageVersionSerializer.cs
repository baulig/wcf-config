//
// MessageVersionModule.cs
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
using System.Xml.Schema;
using System.ServiceModel.Channels;

namespace WCF.Config {

	public class MessageVersionSerializer : ValueSerializer<MessageVersion> {

		public override MessageVersion Deserialize (string text)
		{
			var version = (Version)Enum.Parse (typeof(Version), text);
			switch (version) {
			case Version.None:
				return MessageVersion.None;
			case Version.Soap11:
				return MessageVersion.Soap11;
			case Version.Soap11WSAddressing10:
				return MessageVersion.Soap11WSAddressing10;
			case Version.Soap11WSAddressingAugust2004:
				return MessageVersion.Soap11WSAddressingAugust2004;
			case Version.Soap12:
				return MessageVersion.Soap12;
			case Version.Soap12WSAddressing10:
				return MessageVersion.Soap12WSAddressing10;
			case Version.Soap12WSAddressingAugust2004:
				return MessageVersion.Soap12WSAddressingAugust2004;
			default:
				throw new InvalidOperationException ();
			}
		}

		public override string Serialize (MessageVersion instance)
		{
			if (instance == MessageVersion.None)
				return Version.None.ToString ();
			else if (instance == MessageVersion.Soap11)
				return Version.Soap11.ToString ();
			else if (instance == MessageVersion.Soap11WSAddressing10)
				return Version.Soap11WSAddressing10.ToString ();
			else if (instance == MessageVersion.Soap11WSAddressingAugust2004)
				return Version.Soap11WSAddressingAugust2004.ToString ();
			else if (instance == MessageVersion.Soap12)
				return Version.Soap12.ToString ();
			else if (instance == MessageVersion.Soap12WSAddressing10)
				return Version.Soap12WSAddressing10.ToString ();
			else if (instance == MessageVersion.Soap12WSAddressingAugust2004)
				return Version.Soap12WSAddressingAugust2004.ToString ();
			else
				throw new InvalidOperationException ();
		}

		public override XmlSchemaSimpleType SchemaType {
			get { return Generator.CreateEnumerationType (typeof(Version)); }
		}

		public enum Version {
			None,
			Soap11,
			Soap12WSAddressing10,
			Soap12,
			Soap11WSAddressing10,
			Soap11WSAddressingAugust2004,
			Soap12WSAddressingAugust2004
		}

	}
}

