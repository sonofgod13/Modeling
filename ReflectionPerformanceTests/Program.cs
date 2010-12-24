using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using Helpers;

namespace ReflectionPerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var iterations = 10000000;

            var integers = RandomSequence.GetSequenceOfInts().Take(iterations)
                                                             .Select(i => i.ToString());
            var tests = new[] { 
                new TestEntry(() => RawForeachTest(integers), "Raw foreach"),
                new TestEntry(() => SimpleInvokeTest(integers), "Simple method invoke"),
                new TestEntry(() => ReflectionInvokeTest(integers), "Reflection method invoke"),
                new TestEntry(() => ReflectionWithCacheInvokeTest(integers), "Reflection with cached method invoke"),
                new TestEntry(() => LambdaInvokeTest(integers), "Method invoke by cached compiled lambda"),
                new TestEntry(() => TypeConverterTest(integers), "TypeConverter"),
                new TestEntry(() => TypeConverterCachedTest(integers), "Cached TypeConverter")
            };

            foreach (var entry in tests)
            {
                var time = MeasureTime(entry.TestMethod);
                Console.WriteLine("{0}: {1}", entry.Message, time);
            }

            Console.ReadLine();
        }

        private static void TypeConverterCachedTest(IEnumerable<string> integers)
        {
            var converter = TypeDescriptor.GetConverter(typeof(int));
            foreach (var element in integers)
            {
                var result = converter.ConvertFromString(element);
            }
        }

        private static void TypeConverterTest(IEnumerable<string> integers)
        {
            foreach (var element in integers)
            {
                var converter = TypeDescriptor.GetConverter(typeof(int));
                var result = converter.ConvertFromString(element);
            }
        }

        private static void LambdaInvokeTest(IEnumerable<string> integers)
        {
            foreach (var element in integers)
            {
                var lambda = CompiledLambdaParse.GetParseLambda<int>();
                var result = lambda(element);
            }
        }

        private static void ReflectionWithCacheInvokeTest(IEnumerable<string> integers)
        {
            var type = typeof(int);
            var method = type.GetMethod("Parse", new[] { typeof(String) });

            foreach (var element in integers)
            {
                var result = (int)method.Invoke(
                    null,
                    new object[] { element }
                );
            }
        }

        private static void ReflectionInvokeTest(IEnumerable<string> integers)
        {
            foreach (var element in integers)
            {
                var type = typeof(int);
                var method = type.GetMethod("Parse", new[] { typeof(String) });

                var result = (int)method.Invoke(
                    null,
                    new object[] { element }
                );
            }
        }

        private static void SimpleInvokeTest(IEnumerable<string> integers)
        {
            foreach (var element in integers)
            {
                var result = int.Parse(element);
            }
        }

        private static void RawForeachTest(IEnumerable<string> integers)
        {
            foreach (var element in integers) { }
        }

        static double MeasureTime(Action action)
        {
            DateTime start = DateTime.Now;

            action();

            DateTime end = DateTime.Now;

            return (end - start).TotalMilliseconds;
        }
    }
}
