using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DTALib;
using DTALib.Forms;

namespace AdriansLibClient
{

	class AnOptionClass : OptionSet
	{
		public int AnInt { get; set; }
		public string AString { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AnOptionClass a = obj as AnOptionClass;
			if (a.AnInt != this.AnInt) return false;
			if (!a.AString.Equals(this.AString)) return false;
			return true;
		}


		public static int NullableHashCode(object s)
		{
			if (s == null) return 0;
			return s.GetHashCode();
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return AnInt.GetHashCode() ^ NullableHashCode(AString);
		}
	}

	class AnotherOptionClass : OptionSet
	{
		public double PrivateVal { get; private set; }
		public int PublicInt { get; set; }
		public string PublicString { get; set; }
		public AnOptionClass MoreOptions { get; set; }

		public AnotherOptionClass() {  }

		public AnotherOptionClass(double privateVal)
		{
			this.PrivateVal = privateVal;
		}


		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			AnotherOptionClass a = obj as AnotherOptionClass;
			if (a.PublicInt != this.PublicInt) return false;
			if (!a.PublicString.Equals(this.PublicString)) return false;
			if (!a.MoreOptions.Equals(this.MoreOptions)) return false;
			return base.Equals(obj);
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return PublicInt.GetHashCode() ^ AnOptionClass.NullableHashCode(PublicString) ^ AnOptionClass.NullableHashCode(MoreOptions);
		}
	}

	[TestFixture]
	public class TestCloneableObject
	{
		[Test]
		public void Test()
		{
			AnOptionClass a = new AnOptionClass() { AnInt = 5, AString = "Test" };
			AnOptionClass b = a.Clone() as AnOptionClass;
			Assert.That(b, Is.TypeOf(a.GetType()));
			Assert.That(a.AnInt, Is.EqualTo(b.AnInt));
			Assert.That(a.AString, Is.EqualTo(b.AString));
		}
		[Test]
		public void TestMoreComplexClass()
		{
			AnOptionClass a = new AnOptionClass() { AnInt = 5, AString = "Test" };
			AnotherOptionClass c = new AnotherOptionClass(15.5) { PublicInt = 15, PublicString = "15", MoreOptions = a };
			AnotherOptionClass b = c.Clone() as AnotherOptionClass;
			Assert.That(b, Is.TypeOf(c.GetType()));
			Assert.That(b.PublicInt, Is.EqualTo(c.PublicInt));
			Assert.That(b.PublicString, Is.EqualTo(c.PublicString));
			Assert.That(a.AnInt, Is.EqualTo(b.MoreOptions.AnInt));
			Assert.That(a.AString, Is.EqualTo(b.MoreOptions.AString));
			Assert.That(b.PrivateVal, Is.Not.EqualTo(c.PrivateVal));
		}

		[Test]
		public void TestComplexClassAfterChange()
		{
			AnOptionClass a = new AnOptionClass() { AnInt = 5, AString = "Test" };
			AnotherOptionClass c = new AnotherOptionClass(15.5) { PublicInt = 15, PublicString = "15", MoreOptions = a };
			AnotherOptionClass b = c.Clone() as AnotherOptionClass;
			a.AnInt--;
			a.AString = "different now!";
			c.PublicInt--;
			c.PublicString = "public but different";
			Assert.That(b, Is.TypeOf(c.GetType()));
			Assert.That(b.PublicInt, Is.Not.EqualTo(c.PublicInt));
			Assert.That(b.PublicString, Is.Not.EqualTo(c.PublicString));
			Assert.That(a.AnInt, Is.Not.EqualTo(b.MoreOptions.AnInt));
			Assert.That(a.AString, Is.Not.EqualTo(b.MoreOptions.AString));
		}
	}
}
