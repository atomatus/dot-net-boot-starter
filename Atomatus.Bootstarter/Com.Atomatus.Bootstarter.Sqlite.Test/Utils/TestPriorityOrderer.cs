using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Com.Atomatus.Bootstarter
{
    public class TestPriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sorted = new SortedDictionary<int, List<TTestCase>>();
            
            foreach (TTestCase testCase in testCases)
            {
                int priority = testCase.TestMethod.Method
                    .GetCustomAttributes((typeof(TestPriorityAttribute).AssemblyQualifiedName))
                    .LastOrDefault()?
                    .GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority)) ?? 0;

                if(!sorted.TryGetValue(priority, out var list))
                {
                    sorted[priority] = list = new List<TTestCase>();
                }

                list.Add(testCase);
            }

            foreach (var pair in sorted)
            {
                var list = pair.Value;
                list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
                foreach (TTestCase testCase in list)
                {
                    yield return testCase;
                }
            }
        }

    }
}
