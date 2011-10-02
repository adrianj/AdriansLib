using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexMath
{
	public interface ICFunction
	{
		string Name { get; }
		CDoubleArray Eval();
		List<ICFunction> Children { get; set; }
		bool AlreadyEvaluated { get; set; }
		List<string> GetDependencies();
	}

	/// <summary>
	/// Delegate function which is called if an unknown function label is encountered.
	/// </summary>
	/// <param name="functionDescription"></param>
	/// <returns></returns>
	public delegate CDoubleArray NodeLabelCallback(string label);

	public interface FunctionCallbackInterface
	{
		CDoubleArray FunctionCallback(string label);
	}
}
