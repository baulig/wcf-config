//
// Attribute.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://www.xamarin.com)
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

namespace WCF.Config {

	public abstract class Attribute {
		public string Name {
			get;
			set;
		}

		public bool IsRequired {
			get;
			set;
		}

		public abstract string GetValue (object instance);
	}

	public class Attribute<T> : Attribute
		where T : class
	{
		Func<T, string> func;

		public Attribute (string name, Func<T, string> func)
			: this (name, func, true)
		{ }

		public Attribute (string name, Func<T, string> func, bool required)
		{
			this.Name = name;
			this.IsRequired = required;
			this.func = func;
		}

		public override string GetValue (object instance)
		{
			T value = instance as T;
			if (value == null)
				return null;
			return func (value);
		}
	}
}
