//
// TestModule.cs
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
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Mono.System.ServiceModel.Configuration.Modules {

	public class MyTest : Test {
		public string Hello {
			get; set;
		}

		public double PI {
			get; set;
		}

		public override void Run ()
		{
			Console.WriteLine ("Hello World");
		}
	}

	public class TestCollectionModule : KeyedCollectionModule<Test> {
		
		public override string Name {
			get { return "tests"; }
		}
		
		protected override string KeyName {
			get { return "@name"; }
		}
		
		protected override void Populate ()
		{
			AddElement<MyTestModule, MyTest> ();
			base.Populate ();
		}
		
	}

	public class TestValue : Value<Test> {
		public TestValue ()
		{
			AddAttribute ("name", true, i => i.Name, (i,v) => i.Name = v);
			AddAttribute ("test", i => i.Time, (i,v) => i.Time = v);
		}
	}

	public class MyTestValue : Value<MyTest> {
		public MyTestValue ()
		{
			AddAttribute ("hello", true, i => i.Hello, (i,v) => i.Hello = v);
		}
	}

	public abstract class TestModule<T> : ValueModule<T>
		where T : Test, new()
	{
		protected override void Populate ()
		{
			AddAttribute ("name", true, i => i.Name, (i,v) => i.Name = v);
			AddAttribute ("test", i => i.Time, (i,v) => i.Time = v);
			base.Populate ();
		}
	}

	public class MyTestModule : ValueModule<MyTest> {
		
		public override string Name {
			get { return "test"; }
		}
		
		protected override void Populate ()
		{
			Implement<TestValue> ();
			// Implement<MyTestValue> ();
			AddAttribute ("hello", true, i => i.Hello, (i,v) => i.Hello = v);
			// AddAttribute ("time", i => i.Time, (i,v) => i.Time = v);
			// AddAttribute ("pi", i => i.PI, (i,v) => i.PI = v);
			base.Populate ();
		}
		
	}
}
