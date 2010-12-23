using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace ReflectionPerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var iterations = 10000000;

            var integers = RandomSequence.GetSequenceOfInts().Take(iterations)
                                                             .Select(i => i.ToString());

            var rawForeach = MeasureTime(() =>
            {
                foreach (var element in integers) { }
            });

            Console.WriteLine("Raw foreach: {0}", rawForeach);


            var simpleInvoke = MeasureTime(() =>
            {
                foreach (var element in integers)
                {
                    var result = int.Parse(element);
                }
            });

            Console.WriteLine("Simple method invoke: {0}", simpleInvoke);


            var reflectionInvoke = MeasureTime(() =>
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
                });

            Console.WriteLine("Reflection method invoke: {0}", reflectionInvoke);


            var reflectionWithCacheInvoke = MeasureTime(() =>
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
            });

            Console.WriteLine("Reflection with cached method invoke: {0}", reflectionWithCacheInvoke);


            var lambdaInvoke = MeasureTime(() =>
            {
                foreach (var element in integers)
                {
                    var lambda = GetParseLambda<int>();
                    var result = lambda(element);
                }
            });

            Console.WriteLine("Method invoke by cached compiled lambda: {0}", lambdaInvoke);

            Console.ReadLine();
        }

        static double MeasureTime(Action action)
        {
            DateTime start = DateTime.Now;

            action();

            DateTime end = DateTime.Now;

            return (end - start).TotalMilliseconds;
        }

        static Dictionary<Type, Delegate> CompiledLambdaCache = new Dictionary<Type, Delegate>();

        static Func<String, TTarget> GetParseLambda<TTarget>()
        {
            var targetType = typeof(TTarget);

            if (CompiledLambdaCache.ContainsKey(targetType))
            {
                return (Func<String, TTarget>)CompiledLambdaCache[targetType];
            }

            var parseMethod = targetType.GetMethod(
                "Parse",
                new[] { typeof(String) }
            );

            if (parseMethod == null)
                throw new NotSupportedException();

            var parameter = Expression.Parameter(typeof(String), "value");

            var expression = Expression.Convert(
                Expression.Call(
                    Expression.Constant(null),
                    parseMethod,
                    parameter
                ),
                targetType
            );

            var lambda = Expression.Lambda(expression, parameter).Compile();

            CompiledLambdaCache.Add(targetType, lambda);

            return (Func<String, TTarget>)lambda;
        }
    }
}
