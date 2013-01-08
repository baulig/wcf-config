//
// ListModule.cs
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
using System.ServiceModel.Channels;

namespace WCF.Config {

	public abstract class ListModule<T> : Module
		where T : class
	{
		public override bool IsSupported (object instance)
		{
			return instance is IList<T>;
		}

		protected override void CreateSchema (XmlSchemaComplexType type)
		{
			var all = new XmlSchemaAll ();
			foreach (var child in Children) {
				all.Items.Add (child.CreateSchema ());
			}
			type.Particle = all;
			
			base.CreateSchema (type);
		}
		
		public override bool HasChildren {
			get { return true; }
		}
		
		public override void Serialize (XmlWriter writer, object instance)
		{
			writer.WriteStartElement ("test", Name, Generator.Namespace);
			foreach (var item in (IList<T>)instance) {
				foreach (var child in Children) {
					if (!child.IsSupported (item))
						continue;
					child.Serialize (writer, item);
				}
			}
			writer.WriteEndElement ();
		}
	}
}

