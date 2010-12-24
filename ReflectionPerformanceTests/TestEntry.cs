using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionPerformanceTests
{
    public class TestEntry
    {
        public string Message{get;set;}

        public Action TestMethod { get; set; }

        public TestEntry(Action testMethod, String message)
        {
            this.Message = message;
            this.TestMethod = testMethod;
        }
    }
}
