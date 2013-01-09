//
// AttributeList.cs
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
using System.Collections.Generic;

namespace WCF.Config {

	public class AttributeList<T> : List<Attribute<T>>
		where T : class, new()
	{
		public AttributeList ()
		{
		}

		public Attribute<T> Add<U> (string name, Func<T, U> getter, Action<T, U> setter)
		{
			return Add (name, false, getter, setter);
		}

		public Attribute<T> Add<U> (string name, bool required,
		                         Func<T, U> getter, Action<T, U> setter)
		{
			var attr = new Attribute<T> (name, required, i => {
				return (object)getter (i);
			}, (i,v) => {
				throw new NotImplementedException ();
			});
			base.Add (attr);
			return attr;
		}
	}
}
