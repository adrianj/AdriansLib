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
			ret += TestCompileFromSnippets(ref failCount, ref testCount) + Environment.NewLine;
			return ret;
		}

		private string TestCompileFromSource(ref int failCount, ref int testCount)
		{
			RuntimeCompiler compiler = new RuntimeCompiler();
			List<string> source = new List<string>();
			using (StreamReader reader = new StreamReader(new FileStream("HelloWorld.cs", FileMode.Open)))
				source.Add(reader.ReadToEnd());

			compiler.SourceCode = source;
			testCount++;
			Assembly asm = null;
			string ret = "";
			try { asm = compiler.CompileSource(); }
			catch (ArgumentException ae) { failCount++; ret += ae + "\n"; }
			ret += TestAssembly(ref failCount, ref testCount, asm);
			if (string.IsNullOrWhiteSpace(ret))
				return "No Errors";
			return ret;
		}

		private string TestCompileFromSnippets(ref int failCount, ref int testCount)
		{
			RuntimeCompiler compiler = new RuntimeCompiler();
			string assemblyName = "TestAssembly";
			compiler.AssemblyName = assemblyName;
			List<string> source = new List<string>();
			using (StreamReader reader = new StreamReader(new FileStream("HelloWorld.cs", FileMode.Open)))
				source.Add(reader.ReadToEnd());

			compiler.SourceCode = source;
			testCount++;
			Assembly asm = null;
			string ret = "";
			try { asm = compiler.CompileSourceSnippets(); }
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
