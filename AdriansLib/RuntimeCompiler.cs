using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

namespace DTALib
{
	public class RuntimeCompiler
	{
		CodeDomProvider compiler;
		CompilerParameters parameters;
		int appendedLines = 0;

		public string AssemblyName { get { return parameters.OutputAssembly; } set { parameters.OutputAssembly = value; } }

		private List<string> sourceCode = new List<string>();
		public List<string> SourceCode { get { return sourceCode; } set { sourceCode = value; } }

		public System.Collections.Specialized.StringCollection ReferencedAssemblies { get { return parameters.ReferencedAssemblies; } }

		public RuntimeCompiler()
		{
			compiler = CodeDomProvider.CreateProvider("CSharp");
			parameters = new CompilerParameters();
			AddRunningAssemblies();
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;
			parameters.OutputAssembly = "RuntimeCompiler";
		}

		public void AddTypeToAssembly(Type type)
		{
			string loc = type.Assembly.Location;
			if (!string.IsNullOrWhiteSpace(loc) && !ReferencedAssemblies.Contains(loc))
				ReferencedAssemblies.Add(loc);
		}

		void AddRunningAssemblies()
		{
			List<string> asms = new List<string>();
			asms.Add("System.dll");
			Assembly asm = Assembly.GetExecutingAssembly();
			if (asm != null && !asms.Contains(asm.Location))
				asms.Add(asm.Location);
			asm = Assembly.GetCallingAssembly();
			if (asm != null && !asms.Contains(asm.Location))
				asms.Add(asm.Location);
			asm = Assembly.GetEntryAssembly();
			if (asm != null && !asms.Contains(asm.Location))
				asms.Add(asm.Location);
			this.ReferencedAssemblies.AddRange(asms.ToArray());
		}

		public Assembly CompileSource()
		{
			appendedLines = 0;
			return CompileSource(this.SourceCode);
		}

		public Assembly CompileSourceSnippets()
		{
			List<string> code = new List<string>();
			foreach (string originalCode in this.SourceCode)
			{
				string sc = BuildCodeFromSnippets(originalCode);
				code.Add(sc);
			}
			return CompileSource(code);
		}

		private string BuildCodeFromSnippets(string originalCode)
		{
			List<string> usings = GetReferencedNamespaces();
			StringBuilder source = new StringBuilder();
			foreach (string names in usings)
				source.AppendLine("using " + names + ";");
			source.AppendLine("namespace " + this.AssemblyName + "\n{");
			source.Append(originalCode);
			source.AppendLine("}");
			appendedLines = usings.Count + 2;
			string sc = source.ToString();
			return sc;
		}

		public List<string> GetReferencedNamespaces()
		{
			List<string> usings = new List<string>();
			foreach (Assembly reffedAssembly in GetReferencedAssemblies())
			{
				usings = AppendUsings(usings, reffedAssembly);
			}
			return usings;
		}

		private Assembly LoadAssemblyFromString(string asmRef)
		{
			try { return Assembly.LoadFrom(asmRef); }
			catch (System.IO.FileNotFoundException) { }

			string path = GetAssemblyPath(asmRef);
			return Assembly.LoadFrom(path);
		}

		private List<string> AppendUsings(List<string> usings, Assembly referencedAssembly)
		{
			if (referencedAssembly != null)
				foreach (Type t in referencedAssembly.GetTypes())
				{
					string names = t.Namespace;
					if (!string.IsNullOrWhiteSpace(names) && !usings.Contains(names))
					{
						usings.Add(names);
					}
				}
			return usings;
		}


		Assembly CompileSource(IEnumerable<string> source)
		{
			CompilerResults results = compiler.CompileAssemblyFromSource(parameters, source.ToArray());
			if (results.Errors.HasErrors)
			{
				string err = FormatErrorString(results);
				throw new ArgumentException(err);
			}
			return results.CompiledAssembly;
		}

		private string FormatErrorString(CompilerResults results)
		{
			StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
			foreach (CompilerError error in results.Errors)
			{
				errors.AppendFormat("Line {0},{1}\t: {2}\n",
					   error.Line - appendedLines, error.Column, error.ErrorText);
			}
			string err = errors.ToString();
			return err;
		}

		public List<Assembly> GetReferencedAssemblies()
		{
			List<Assembly> ret = new List<Assembly>();
			foreach (string asmRef in this.ReferencedAssemblies)
			{
				Assembly reffedAssembly = LoadAssemblyFromString(asmRef);
				if (reffedAssembly != null)
					ret.Add(reffedAssembly);
			}
			return ret;
		}



		/// <summary>
		/// Gets an assembly path from the GAC given a partial name.
		/// </summary>
		/// <param name="name">An assembly partial name. May not be null.</param>
		/// <returns>
		/// The assembly path if found; otherwise null;
		/// </returns>
		public static string GetAssemblyPath(string name)
		{
			if (name.EndsWith(".dll") || name.EndsWith(".exe"))
				name = name.Substring(0, name.Length - 4);
			if (name == null)
				throw new ArgumentNullException("name");

			string finalName = name;
			AssemblyInfo aInfo = new AssemblyInfo();
			aInfo.cchBuf = 1024; // should be fine...
			aInfo.currentAssemblyPath = new String('\0', aInfo.cchBuf);

			IAssemblyCache ac;
			int hr = CreateAssemblyCache(out ac, 0);
			if (hr >= 0)
			{
				hr = ac.QueryAssemblyInfo(0, finalName, ref aInfo);
				if (hr < 0)
					return null;
			}

			return aInfo.currentAssemblyPath;
		}


		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
		private interface IAssemblyCache
		{
			void Reserved0();

			[PreserveSig]
			int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct AssemblyInfo
		{
			public int cbAssemblyInfo;
			public int assemblyFlags;
			public long assemblySizeInKB;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string currentAssemblyPath;
			public int cchBuf; // size of path buf.
		}

		[DllImport("fusion.dll")]
		private static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

	}
}
