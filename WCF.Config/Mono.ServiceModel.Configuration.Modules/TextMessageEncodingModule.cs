//
// TextMessageEncodingModule.cs
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

namespace Mono.ServiceModel.Configuration.Modules {

	class TextMessageEncodingModule : ValueModule<TextMessageEncodingBindingElement> {

		public override string Name {
			get { return "textMessageEncoding"; }
		}

		protected override void Populate ()
		{
			AddAttribute (
				"messageVersion", i => i.MessageVersion, (i,v) => i.MessageVersion = v).
				SetCustomSerializer<MessageVersionSerializer> ();
			AddAttribute (
				"maxReadPoolSize", i => i.MaxReadPoolSize, (i,v) => i.MaxReadPoolSize = v).
				SetMinMax ("1", int.MaxValue.ToString ());
			AddAttribute (
				"maxWritePoolSize", i => i.MaxWritePoolSize, (i,v) => i.MaxWritePoolSize = v).
				SetMinMax ("1", int.MaxValue.ToString ());
			AddAttribute (
				"writeEncoding", i => i.WriteEncoding, (i,v) => i.WriteEncoding = v);

			base.Populate ();
		}
	}
}

