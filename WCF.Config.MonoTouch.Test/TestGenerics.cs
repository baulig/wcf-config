//
// TestGenerics.cs
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

namespace WCF.Config.MonoTouch.Test {

	using C = Mono.System.ServiceModel.Configuration;
	using M = Mono.System.ServiceModel.Configuration.Module;

	public class TestGenerics {

		public static void TestLambda<T> (Func<T> func)
		{
			T value = func ();
			Console.WriteLine (value);
		}

		public abstract class Attribute<T> {
		}

		public class Attribute<T,U> : Attribute<T>
			where T : class, new()
		{
		}

		public abstract class Module<T>
			where T : class, new()
		{

			public C.Attribute<T,U> AddAttribute<U> (string name, Func<T, U> getter, Action<T, U> setter)
			{
				return new C.Attribute<T, U> (name, getter, setter);
			}

		}

		public class Foo {
			public string Name {
				get; set;
			}
		}

		public class FooModule : Module<Foo>
		{
			public void Test ()
			{
				AddAttribute ("test", i => i.Name, (i,v) => i.Name = v);
			}
		}

		public class TestModule : C.ValueModule<Foo>
		{
			public override string Name {
				get { return "test"; }
			}

			protected override void Populate ()
			{
				Console.WriteLine ("POPULATE TEST");
				AddAttribute ("test", i => i.Name, (i,v) => i.Name = v);
				base.Populate ();
			}
		}

		public static void Run ()
		{
			TestLambda<int> (() => 9);
			TestLambda (() => Math.PI);

			var module = new FooModule ();
			module.Test ();

			var test = new TestModule ();
		}

	}
}

