using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DTALib
{

    public interface ITestClass
    {
        /// <summary>
        /// Runs the tests associated with this class.
        /// </summary>
        /// <param name="failCount">Reference to an integer containing the running total of test failures.</param>
        /// <param name="testCount">Reference to an integer containing the running total of tests performed</param>
        /// <returns>A string describing the nature of failed tests.</returns>
        string RunTests(ref int failCount, ref int testCount);
    }

    /// <summary>
    /// Sets up a pattern for testing classes. To test a class:
    /// 1. Write another class that implements ITestClass, with default constructor.
    /// 2. Implement RunTests() which excersises the class to test.
    /// 3. In main method, call Tester.RunTests, optionally supplying the names of the classes that implement ITestClass.
    /// </summary>
    public class Tester
    {

        public static int RunTests(string[] args)
        {
            int failures = 0;
            Assembly asm = Assembly.GetCallingAssembly();
            // Test code as determined by args. if no args are specified then it runs all the tests.
            // The static method that is called is named "Test_<args[i]>", so if no method exists then no biggie.
            if (args.Length == 0) // Run all tests
            {
                foreach (Type type in asm.GetTypes())
                {
                    // Find and run types that implement TestClass interface
                    if (type.GetInterfaces().Contains(typeof(ITestClass)))
                    {
                        ITestClass tester = (ITestClass)Activator.CreateInstance(type);
                        int failCount = 0, testCount = 0;
                        string failMsg = tester.RunTests(ref failCount, ref testCount);
                        failures += failCount;
                    }
                }
            }
            else
            {
                foreach (string arg in args)
                {
                    foreach (Type type in asm.GetTypes())
                    {
                        string n = type.Name;
                        if (n.Length < arg.Length) continue;
                        n = n.Substring(n.Length - arg.Length);
                        // Find and run types that implement TestClass interface, and name ends with args[i]
                        if (type.GetInterfaces().Contains(typeof(ITestClass)) && n.Equals(arg, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ITestClass tester = (ITestClass)Activator.CreateInstance(type);
                            int failCount = 0, testCount = 0;
                            string failMsg = tester.RunTests(ref failCount, ref testCount);
                            failures += failCount;
                        }
                    }
                }
            }
            return failures;
        }
    }
}
