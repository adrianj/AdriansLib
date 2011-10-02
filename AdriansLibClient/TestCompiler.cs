using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using DTALib;

namespace AdriansLibClient
{
	public class TestCompiler : ITestClass
	{
		public string RunTests(ref int failCount, ref int testCount)
		{
			string ret = "";
			ret += TestCompileFromSource(ref failCount, ref testCount) + Environment.NewLine;
			ret += TestCompileFromSnippet(ref failCount, ref testCount) + Environment.NewLine;
			return ret;
		}

		private string TestCompileFromSource(ref int failCount, ref int testCount)
		{
			RuntimeCompiler compiler = new RuntimeCompiler();
			using (StreamReader reader = new StreamReader(new FileStream("HelloWorld.cs", FileMode.Open)))
				compiler.SourceCode = reader.ReadToEnd();
			testCount++;
			Assembly asm = null;
			string ret = "";
			try { asm = compiler.CompileSourceFile(); }
			catch (ArgumentException ae) { failCount++; ret += ae + "\n"; }
			ret += TestAssembly(ref failCount, ref testCount, asm);
			if (string.IsNullOrWhiteSpace(ret))
				return "No Errors";
			return ret;
		}

		private string TestCompileFromSnippet(ref int failCount, ref int testCount)
		{
			RuntimeCompiler compiler = new RuntimeCompiler();
			string assemblyName = "TestAssembly";
			compiler.AssemblyName = assemblyName;
			using (StreamReader reader = new StreamReader(new FileStream("HelloWorldSnippet.txt", FileMode.Open)))
				compiler.SourceCode = reader.ReadToEnd();
			testCount++;
			Assembly asm = null;
			string ret = "";
			try { asm = compiler.CompileSourceSnippet(); }
			catch (ArgumentException ae) { failCount++; ret += ae + "\n"; }
			ret += TestAssembly(ref failCount, ref testCount, asm);
			if (string.IsNullOrWhiteSpace(ret))
				return "No Errors";
			return ret;
		}

		private string TestAssembly(ref int failCount, ref int testCount, Assembly asm)
		{
			Console.WriteLine("" + asm);
			string ret = "";
			if (asm != null)
			{
				foreach (Type t in asm.GetTypes())
				{
					if (!t.IsAbstract && !t.IsInterface)
					{
						Console.WriteLine("Calling default constructor of class '" + t + "'");
						testCount++;
						try { Activator.CreateInstance(t); }
						catch (Exception ex) { failCount++; ret += "" + ex + "\n"; }
					}
				}
			}
			return ret;
		}

		
	}
}
