using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DTALib.Forms;
using System.Windows.Forms;

namespace AdriansLibClient
{

	[TestFixture]
	public class Test_OptionsDialog
	{

		static AnOptionClass a = new AnOptionClass() { AnInt = 5, AString = "Test", OptionsLabel = "Option Set A" };
		AnOptionClass b = new AnOptionClass() { AnInt = 15, AString = "test 15", OptionsLabel = "Option Set B" };
		AnOptionClass c = new AnOptionClass() { AnInt = 25, AString = "test 25", OptionsLabel = "Option Set C" };
		AnotherOptionClass d = new AnotherOptionClass() { PublicInt = 22, PublicString = "test 22", OptionsLabel = "Complex Set D" };
		AnotherOptionClass e = new AnotherOptionClass() { PublicInt = 22, PublicString = "test 33", OptionsLabel = "Complex Set E" };

		[SetUp]
		public void SetUp()
		{
			d.MoreOptions = new AnOptionClass() { OptionsLabel = "SubSet 1" };
		}

		[Test]
		public void TestWithNoOptions()
		{
			List<ICloneable> options = new List<ICloneable>();
			DialogResult r = OptionsDialog.Show(options);
			Console.WriteLine("" + r);
		}

		[Test]
		public void TestWithOneOption()
		{
			List<ICloneable> options = new List<ICloneable>();
			options.Add(new AnOptionClass() { AnInt = 5, AString = "Test" });
			DialogResult r = OptionsDialog.Show(options);
			Console.WriteLine("" + r);
		}

		[Test]
		public void TestWithMultipleOption()
		{
			List<ICloneable> options = new List<ICloneable>();
			options.Add(a);
			options.Add(b);
			DialogResult r = OptionsDialog.Show(options);
			Console.WriteLine("" + r);
		}

		[Test]
		public void TestWithComplexOption()
		{
			List<ICloneable> options = new List<ICloneable>();
			options.Add(a);
			options.Add(b);
			options.Add(d);
			options.Add(e);
			DialogResult r = OptionsDialog.Show(options);
			if (r == DialogResult.Cancel)
			{
				Assert.That(options[0], Is.EqualTo(a));
				Assert.That(options[1], Is.EqualTo(b));
				Assert.That(options[2], Is.EqualTo(d));
				Assert.That(options[3], Is.EqualTo(e));
			}
			foreach (OptionSet.ModifiedDataPair m in (options[0] as OptionSet).GetModifiedValues(a))
				Console.WriteLine("" + m);
			Console.WriteLine();
			foreach (OptionSet.ModifiedDataPair m in (options[2] as OptionSet).GetModifiedValues(d))
				Console.WriteLine("" + m);
			Console.WriteLine();
			foreach (OptionSet.ModifiedDataPair m in (options[3] as OptionSet).GetModifiedValues(e))
				Console.WriteLine("" + m);
		}
	}
}
